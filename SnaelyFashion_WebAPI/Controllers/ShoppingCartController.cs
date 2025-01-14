﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;
using SnaelyFashion_Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using SnaelyFashion_Models.DTO.ShoppingCart_;
using SnaelyFashion_Models.DTO.OrderHeaderDTO_;
using SnaelyFashion_Models.DTO.Profile_;
using SnaelyFashion_Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using SnaelyFashion_WebAPI.Migrations;
using Stripe.Checkout;


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

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == userId);
                IEnumerable<ProductImage> productImages =await _unitOfWork.ProductImage.GetAllAsync();
                var shoppingcartlistfromDB = await _unitOfWork.ShoppingCart.GetAllAsync(u => u.ApplicationUserId == userId,
                includeProperties: "Product");

                if (shoppingcartlistfromDB == null) 
                {
                    _response.IsSuccess = true;
                    _response.Result = "";
                    _response.StatusCode = System.Net.HttpStatusCode.OK;
                }

                foreach(var item in shoppingcartlistfromDB) 
                {
                    var shoppingcartitemDTO = new ShoppingCartItemDTO
                    {
                        ProductName =item.Product.Title,
                        ShoppingCartId = item.Id,
                        ApplicationUserId = userId,
                        Color = item.Color,
                        Count = item.Count,
                        Price = item.Price,
                        ProductId = item.ProductId,
                        Size = item.Size,
                        ImageUrl = productImages.Where(x=>x.ProductId==item.ProductId).FirstOrDefault()?.ImageUrl
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

        [HttpPut("{ShoppingCartItemId:int}", Name = "EditCartItemQuantity")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<APIResponse> EditCartItemQuantity(int ShoppingCartItemId,int NewCount)
        {
           
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == userId);
                var shoppingcartfromDB = await _unitOfWork.ShoppingCart.GetAsync(u => u.Id == ShoppingCartItemId,
              includeProperties: "Product");

                if (shoppingcartfromDB == null)
                {
                    _response.IsSuccess = false;
                    
                    _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                }

                shoppingcartfromDB.Count = NewCount;
                _unitOfWork.ShoppingCart.Update(shoppingcartfromDB);
                _unitOfWork.Save();
                var ShopingcartItemDTO = new ShoppingCartItemDTO 
                {
                    ProductName = shoppingcartfromDB.Product.Title,
                    ProductId = shoppingcartfromDB.ProductId,
                    ShoppingCartId = ShoppingCartItemId,
                    ApplicationUserId=userId,
                    Color=shoppingcartfromDB.Color,
                    Count=NewCount,
                    Price=shoppingcartfromDB.Price,
                    Size=shoppingcartfromDB.Size,
                    ImageUrl=shoppingcartfromDB.Product.ProductImages.FirstOrDefault()?.ImageUrl
                    

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


        [HttpDelete("{ShoppingCartItemId:int}",Name = "RemoveItemFromShoppingCart")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<APIResponse> RemoveItemFromShoppingCart(int ShoppingCartItemId)
        {

            try
            {
                var itemtodelete = await _unitOfWork.ShoppingCart.GetAsync(x=>x.Id == ShoppingCartItemId);
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
                 var shoppingCartDTO = new ShoppingCartDTO();

                var shoppingcartitemDTOlist = new List<ShoppingCartItemDTO>();

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
                    var shoppingcartitemDTO = new ShoppingCartItemDTO
                    {
                        ProductName = item.Product.Title,
                        ShoppingCartId = item.Id,
                        ApplicationUserId = userId,
                        Color = item.Color,
                        Count = item.Count,
                        Price = item.Price,
                        ProductId = item.ProductId,
                        Size = item.Size,
                        ImageUrl = productImages.Where(x => x.ProductId == item.ProductId).FirstOrDefault()?.ImageUrl
                    };
                    ordertotal += item.Count * item.Price;
                    shoppingcartitemDTOlist.Add(shoppingcartitemDTO);

                }
                shoppingCartDTO.ShoppingCartList=shoppingcartitemDTOlist;

                var orderheaderDTO = new OrderHeaderDetailsDTO
                {
                    ApplicationUserId = userId,
                    PhoneNumber = user.PhoneNumber,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    State = user.State,
                    City=user.City,
                    PostalCode=user.PostalCode,
                    StreetAddress=user.StreetAddress,
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
        public async Task<ActionResult<APIResponse>> SummaryPost(ShoppingCartDTO shoppingCartDTO,string PaymentMethod)
        {
            

            try
            {
                

                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                var user = await _unitOfWork.ApplicationUser.GetAsync(x => x.Id == userId);
               
                OrderHeader neworderheader = new OrderHeader(); 

                neworderheader.OrderDate = DateTime.Now;
                neworderheader.ApplicationUserId = userId;
                neworderheader.OrderTotal = shoppingCartDTO.OrderHeaderDetails.OrderTotal;
                neworderheader.PhoneNumber = shoppingCartDTO.OrderHeaderDetails.PhoneNumber;
                neworderheader.FirstName=shoppingCartDTO.OrderHeaderDetails.FirstName;
                neworderheader.LastName=shoppingCartDTO.OrderHeaderDetails.LastName;
                neworderheader.State = shoppingCartDTO.OrderHeaderDetails.State;
                neworderheader.City = shoppingCartDTO.OrderHeaderDetails.City;
                neworderheader.StreetAddress = shoppingCartDTO.OrderHeaderDetails.StreetAddress;
                neworderheader.PostalCode = shoppingCartDTO.OrderHeaderDetails.PostalCode;

                if (PaymentMethod==SD.PaymentMethod_Cash) 
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

                var createdorderheader = await _unitOfWork.OrderHeader.GetAsync(x=>x.ApplicationUserId==userId&&x.OrderTotal==neworderheader.OrderTotal);

                foreach (var cart in shoppingCartDTO.ShoppingCartList)
                {
                    OrderDetail orderDetail = new()
                    {
                        ProductId = cart.ProductId,
                        OrderHeaderId = createdorderheader.Id,
                        Price = cart.Price,
                        Count = cart.Count
                    };
                    _unitOfWork.OrderDetail.Add(orderDetail);
                    _unitOfWork.Save();
                }


                shoppingCartDTO.OrderHeaderId = createdorderheader.Id;


                if (PaymentMethod==SD.PaymentMethod_Card)
                {
                    var allproducts = await _unitOfWork.Product.GetAllAsync();


                    
                    //stripe logic
                    var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                    var options = new SessionCreateOptions
                    {
                        SuccessUrl = domain + $"www.google.com",
                        CancelUrl = domain + "www.youtube.com",
                        LineItems = new List<SessionLineItemOptions>(),
                        Mode = "payment",
                    };

                    foreach (var item in shoppingCartDTO.ShoppingCartList)
                    {
                        var sessionLineItem = new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.ProductName
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
                   
                    _response.IsSuccess = true;
                    _response.Result = shoppingCartDTO;
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


        [HttpDelete("{OrderHeaderId:int}", Name = "OrderConfirmation")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<APIResponse> OrderConfirmation(int OrderHeaderId) 
        {
            try 
            { 

              OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderHeaderId, includeProperties: "ApplicationUser");
              if (orderHeader.PaymentStatus != SD.PaymentMethod_Cash)
              {
                

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentIDAsync(OrderHeaderId, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatusAsync(OrderHeaderId, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
                HttpContext.Session.Clear();

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

            _response.IsSuccess=true;
            _response.Result = $"Order Confirmed {OrderHeaderId}";
            _response.StatusCode=System.Net.HttpStatusCode.OK;

            return _response;


        }






    }
}
