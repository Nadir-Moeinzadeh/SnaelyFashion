using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnaelyFashion_Models.DTO.OrderHeaderDTO_;
using SnaelyFashion_Models.DTO.ShoppingcartDTO_;
using SnaelyFashion_Models.DTO.Summary_;
using SnaelyFashion_Models;
using SnaelyFashion_Utility;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;
using Stripe.Checkout;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Text;

namespace SnaelyFashion_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        protected APIResponse _response;


        public ShoppingCartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _response = new APIResponse();
        }





        [HttpPost("{productId:int}", Name = "AddToCart")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> AddToCart(int productId, [FromBody] ShoppingCartItemAddDTO cartDTO)
        {
            try
            {
                if (productId==0) 
                {
                    _response.StatusCode=HttpStatusCode.BadRequest;
                    _response.IsSuccess=false;
                }
                var product = await _unitOfWork.Product.GetAsync(x=>x.Id == productId);
                if (product == null) 
                {
                    _response.StatusCode=HttpStatusCode.NotFound;
                    _response.IsSuccess=false;
                    return _response;
                
                }
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == userId && u.ProductId == productId);


                if (cartFromDb != null)
                {
                    //shopping cart exists
                    cartFromDb.Count += 1;
                    cartFromDb.Color = cartDTO.Color;
                    cartFromDb.Size = cartDTO.Size;
                    cartFromDb.Price = product.Price;
                    
                    _unitOfWork.ShoppingCart.Update(cartFromDb);
                    _unitOfWork.Save();
                }
                else
                {
                    ShoppingCart newshoppingcart = new ShoppingCart
                    {
                        ApplicationUserId = userId,
                        ProductId = productId,
                        Size = cartDTO.Size,
                        Color = cartDTO.Color,
                        Count = 1,
                        Price = product.Price,

                    };


                    //add cart record
                    _unitOfWork.ShoppingCart.Add(newshoppingcart);
                    _unitOfWork.Save();
                    HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
                }

                _response.Result = cartDTO;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }












        [HttpGet("GetShoppingCartInfo")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetShoppingCartInfo()
        {

            try
            {

                var shoppingcartitemDTOlist = new List<ShoppingCartItemDTO>();
                var allproducts =await _unitOfWork.Product.GetAllAsync();
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == userId);
                IEnumerable<ProductImage> productImages = await _unitOfWork.ProductImage.GetAllAsync();
                var shoppingcartlistfromDB = await _unitOfWork.ShoppingCart.GetAllAsync(u => u.ApplicationUserId == userId,
                includeProperties: "Product");

                if (shoppingcartlistfromDB == null)
                {
                    _response.IsSuccess = true;
                    _response.Result = "";
                    _response.StatusCode = System.Net.HttpStatusCode.OK;
                }

                foreach (var item in shoppingcartlistfromDB)
                {
                    var product = allproducts.Where(x=>x.Id == item.ProductId).FirstOrDefault();

                    var shoppingcartitemDTO = new ShoppingCartItemDTO
                    {
                        ProductName = product.Title,
                        ShoppingCartId = item.Id,
                        ApplicationUserId = userId,
                        Color = item.Color,
                        Count = item.Count,
                        Price = item.Price,
                        ProductId = item.ProductId,
                        Size = item.Size,
                        ImageUrl =SD.Defaultwwwroot+ productImages.Where(x => x.ProductId == item.ProductId).FirstOrDefault()?.ImageUrl
                    };

                    shoppingcartitemDTOlist.Add(shoppingcartitemDTO);

                }



                _response.IsSuccess = true;
                _response.Result = shoppingcartitemDTOlist;
                _response.StatusCode = System.Net.HttpStatusCode.OK;



                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }

        [HttpPut(Name = "EditCartItemQuantity")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<APIResponse> EditCartItemQuantity([FromQuery]int id, [FromQuery]int NewCount)
        {

            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == userId);
                var shoppingcartfromDB = await _unitOfWork.ShoppingCart.GetAsync(u => u.Id == id,
              includeProperties: "Product");

                var product = await _unitOfWork.Product.GetAsync(x => x.Id == shoppingcartfromDB.ProductId);
                var productimage = await _unitOfWork.ProductImage.GetAsync(x=>x.ProductId==product.Id);

                if (shoppingcartfromDB == null)
                {
                    _response.IsSuccess = false;

                    _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                }

                shoppingcartfromDB.Count = NewCount;
                _unitOfWork.ShoppingCart.Update(shoppingcartfromDB);
                _unitOfWork.Save();
                var cartfromDbafterUpdate = await _unitOfWork.ShoppingCart.GetAsync(x => x.Id == id);
                var ShopingcartItemDTO = new ShoppingCartItemDTO
                {
                    ProductName = product.Title,
                    ProductId = cartfromDbafterUpdate.ProductId,
                    ShoppingCartId = id,
                    ApplicationUserId = userId,
                    Color = cartfromDbafterUpdate.Color,
                    Count = cartfromDbafterUpdate.Count,
                    Price = cartfromDbafterUpdate.Price,
                    Size = cartfromDbafterUpdate.Size,
                    ImageUrl =SD.Defaultwwwroot+productimage.ImageUrl


                };



                _response.IsSuccess = true;
                _response.Result = ShopingcartItemDTO;
                _response.StatusCode = System.Net.HttpStatusCode.OK;



                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _response;

        }


        [HttpDelete( Name = "RemoveItemFromShoppingCart")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<APIResponse> RemoveItemFromShoppingCart([FromQuery]int id)
        {

            try
            {
                var itemtodelete = await _unitOfWork.ShoppingCart.GetAsync(x => x.Id == id);
                if (itemtodelete == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                }
                _unitOfWork.ShoppingCart.Remove(itemtodelete);
                _unitOfWork.Save();
                _response.IsSuccess = true;
                _response.Result = "Item Removed";
                _response.StatusCode = System.Net.HttpStatusCode.NoContent;

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }

            return _response;
        }



        [HttpGet("Summary")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> Summary()
        {
            double ordertotal = 0;

            try
            {
                var shoppingCartDTO = new SummaryGetDTO();

                var shoppingcartitemDTOlist = new List<ShoppingCartItemDTO>();
                var AllProducts = await _unitOfWork.Product.GetAllAsync();
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == userId);
                IEnumerable<ProductImage> productImages = await _unitOfWork.ProductImage.GetAllAsync();
                var shoppingcartlistfromDB = await _unitOfWork.ShoppingCart.GetAllAsync(u => u.ApplicationUserId == userId,
                includeProperties: "Product");

                if (shoppingcartlistfromDB == null)
                {
                    _response.IsSuccess = true;
                    _response.Result = "";
                    _response.StatusCode = System.Net.HttpStatusCode.OK;
                }

                foreach (var item in shoppingcartlistfromDB)
                {
                    var product = AllProducts.Where(x=>x.Id==item.ProductId).FirstOrDefault();
                    var shoppingcartitemDTO = new ShoppingCartItemDTO
                    {
                        ProductName = product.Title,
                        ShoppingCartId = item.Id,
                        ApplicationUserId = userId,
                        Color = item.Color,
                        Count = item.Count,
                        Price = item.Price,
                        ProductId = item.ProductId,
                        Size = item.Size,
                        ImageUrl =SD.Defaultwwwroot+ productImages.Where(x => x.ProductId == item.ProductId).FirstOrDefault()?.ImageUrl
                    };
                    ordertotal += item.Count * item.Price;
                    shoppingcartitemDTOlist.Add(shoppingcartitemDTO);

                }
                shoppingCartDTO.ShoppingCartList = shoppingcartitemDTOlist;

                var orderheaderDTO = new OrderHeaderDetailsDTO
                {
                   
                    
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    State = user.State,
                    City = user.City,
                    PostalCode = user.PostalCode,
                    StreetAddress = user.StreetAddress,
                    OrderTotal = ordertotal

                };
                shoppingCartDTO.OrderHeaderDetails = orderheaderDTO;

                _response.IsSuccess = true;
                _response.Result = shoppingCartDTO;
                _response.StatusCode = System.Net.HttpStatusCode.OK;



                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }


        [HttpPost("SummaryPost")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status303SeeOther)]
        public async Task<ActionResult<APIResponse>> SummaryPost(OrderHeaderDetailsDTO orderheaderDetailsDTO, string PaymentMethod)
        {
            double ordertotal = 0;

            try
            {
                var shoppingcartitemDTOlist = new List<ShoppingCartItemDTO>();
                IEnumerable<ProductImage> productImages = await _unitOfWork.ProductImage.GetAllAsync();
                var allproducts = await _unitOfWork.Product.GetAllAsync();
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == userId);
                var shoppingcartslist = await _unitOfWork.ShoppingCart.GetAllAsync(x=>x.ApplicationUserId==userId);
              
                if (shoppingcartslist == null) 
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return _response;
                } 

                OrderHeader neworderheader = new OrderHeader();

                neworderheader.OrderDate = DateTime.Now;
                neworderheader.ApplicationUserId = userId;
                neworderheader.OrderTotal = orderheaderDetailsDTO.OrderTotal;
                neworderheader.PhoneNumber = orderheaderDetailsDTO.PhoneNumber;
                neworderheader.FirstName = orderheaderDetailsDTO.FirstName;
                neworderheader.LastName = orderheaderDetailsDTO.LastName;
                neworderheader.State = orderheaderDetailsDTO.State;
                neworderheader.City = orderheaderDetailsDTO.City;
                neworderheader.StreetAddress = orderheaderDetailsDTO.StreetAddress;
                neworderheader.PostalCode = orderheaderDetailsDTO.PostalCode;

                if (PaymentMethod == SD.PaymentMethod_Cash)
                {
                    neworderheader.PaymentStatus = SD.PaymentMethod_Cash;
                    neworderheader.OrderStatus = SD.StatusApproved;
                }
                if (PaymentMethod == SD.PaymentMethod_Card)
                {
                    neworderheader.PaymentStatus = SD.PaymentStatusPending;
                    neworderheader.OrderStatus = SD.StatusApproved;
                }

                _unitOfWork.OrderHeader.Add(neworderheader);
                _unitOfWork.Save();

                var createdorderheader = await _unitOfWork.OrderHeader.GetAsync(x => x.ApplicationUserId == userId && x.OrderTotal == neworderheader.OrderTotal);

                foreach (var cart in shoppingcartslist)
                {
                    OrderDetail orderDetail = new()
                    {
                        ProductId = cart.ProductId,
                        OrderHeaderId = createdorderheader.Id,
                        Price = cart.Price,
                        Count = cart.Count,
                        Color = cart.Color,
                        Size = cart.Size
                        
                        
                    };
                    _unitOfWork.OrderDetail.Add(orderDetail);
                    _unitOfWork.Save();
                }
               var shoppingCartDTO = new ShoppingCartDTO();






                var shoppingcartlistfromDB = await _unitOfWork.ShoppingCart.GetAllAsync(u => u.ApplicationUserId == userId,
includeProperties: "Product");

                if (shoppingcartlistfromDB == null)
                {
                    _response.IsSuccess = true;
                    _response.Result = "";
                    _response.StatusCode = System.Net.HttpStatusCode.OK;
                }

                foreach (var item in shoppingcartlistfromDB)
                {
                    var product = allproducts.Where(x => x.Id == item.ProductId).FirstOrDefault();
                    var shoppingcartitemDTO = new ShoppingCartItemDTO
                    {
                        ProductName = product.Title,
                        ShoppingCartId = item.Id,
                        ApplicationUserId = userId,
                        Color = item.Color,
                        Count = item.Count,
                        Price = item.Price,
                        ProductId = item.ProductId,
                        Size = item.Size,
                        ImageUrl = SD.Defaultwwwroot + productImages.Where(x => x.ProductId == item.ProductId).FirstOrDefault()?.ImageUrl
                    };
                    ordertotal += item.Count * item.Price;
                    shoppingcartitemDTOlist.Add(shoppingcartitemDTO);

                }
                shoppingCartDTO.ShoppingCartList = shoppingcartitemDTOlist;

                var orderheaderDTO = new OrderHeaderDetailsDTO
                {


                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    State = user.State,
                    City = user.City,
                    PostalCode = user.PostalCode,
                    StreetAddress = user.StreetAddress,
                    OrderTotal = ordertotal

                };
                shoppingCartDTO.OrderHeaderDetails = orderheaderDTO;

                shoppingCartDTO.OrderHeaderId = createdorderheader.Id;
                


                if (PaymentMethod == SD.PaymentMethod_Card)
                {
                   



                    //stripe logic
                    var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                    var options = new SessionCreateOptions
                    {
                        //SuccessUrl = domain + $"www.google.com",
                        //CancelUrl = domain + "www.youtube.com",
                        SuccessUrl =  $"{domain}www.google.com",
                        CancelUrl =  $"{domain}www.youtube.com",
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                    };

                    foreach (var item in shoppingcartslist)
                    {
                        var productinfo = allproducts.Where(x=>x.Id == item.ProductId).FirstOrDefault();

                        var sessionLineItem = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = productinfo.Title
                                }
                            },
                            Quantity = item.Count
                        };
                        options.LineItems.Add(sessionLineItem);
                    }


                    var service = new SessionService();
                    Session session = service.Create(options);
                    _unitOfWork.OrderHeader.UpdateStripePaymentIDAsync(createdorderheader.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                    Response.Headers.Add("Location", session.Url);
                    var result = new StripeSummeryResponseDTO
                    { 
                        CheckoutURL = session.Url,
                        OrderHeaderID = shoppingCartDTO.OrderHeaderId,
                        SessionID = session.Id,
                    };
                        
                   
                    _response.IsSuccess = true;
                    //_response.Result = orderheaderDetailsDTO;
                    _response.Result = result;
                    _response.StatusCode = System.Net.HttpStatusCode.SeeOther;
                }
                if (PaymentMethod == SD.PaymentMethod_Cash)
                {

                    _response.IsSuccess = true;
                    _response.Result = shoppingCartDTO;
                    _response.StatusCode = System.Net.HttpStatusCode.OK;
                }


                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }


        [HttpPut("{OrderHeaderId:int}", Name = "OrderConfirmation")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<APIResponse> OrderConfirmation(int OrderHeaderId,string? sessionID)
        {
            try
            {

                OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderHeaderId, includeProperties: "ApplicationUser");
                if (sessionID !=null && sessionID  !=""&& sessionID!= "string") 
                { 
                  if (orderHeader.PaymentStatus != SD.PaymentMethod_Cash)
                  {


                    var service = new SessionService();
                    Session session = service.Get(sessionID);

                    if (session.PaymentStatus.ToLower() == "paid")
                    {
                        _unitOfWork.OrderHeader.UpdateStripePaymentIDAsync(OrderHeaderId, session.Id, session.PaymentIntentId);
                        _unitOfWork.OrderHeader.UpdateStatusAsync(OrderHeaderId, SD.StatusApproved, SD.PaymentStatusApproved);
                        _unitOfWork.Save();
                    }
                    HttpContext.Session.Clear();

                  }
                }

                _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - SnaelyFashion",
                 $"<p>New Order Created - {orderHeader.Id}</p>");

                List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId).ToList();

                _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
                _unitOfWork.Save();

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages
                     = new List<string>() { ex.ToString() };
            }

            _response.IsSuccess = true;
            _response.Result = $"Order Confirmed {OrderHeaderId}";
            _response.StatusCode = System.Net.HttpStatusCode.OK;

            return _response;


        }






    }
}
