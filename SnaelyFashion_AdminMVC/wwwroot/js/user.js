﻿var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/user/getall' },
        "columns": [
            
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
           
            { "data": "role", "width": "15%" },
            {
                data: { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                        return `
                        <div class="text-center">
                             <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:100px;">
                                    <i class="bi bi-lock-fill"></i>  Lock
                                </a>
                                  <a href="user/RoleManagment?userId=${data.id}" class="btn btn-primary text-white" style="cursor:pointer; width:150px;">
                                     <i class="bi bi-pencil-square"></i> Permission
                                </a>
                                <a onclick=Delete('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                    <i class="bi bi-lock-fill"></i>  Delete
                                </a> 
                               
                              
                        </div>
                    `
                    }
                    else {
                        return `
                        <div class="text-center">
                              <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width:100px;">
                                    <i class="bi bi-unlock-fill"></i>  UnLock
                                </a>
                                  <a href="user/RoleManagment?userId=${data.id}" class="btn btn-primary text-white" style="cursor:pointer; width:150px;">
                                     <i class="bi bi-pencil-square"></i> Permission
                                </a>
                                <a onclick=Delete('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                    <i class="bi bi-lock-fill"></i>  Delete
                                </a> 
                               
                               
                        </div>
                    `
                    }


                },
                "width": "55%"
            }
        ]
    });
}


function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/User/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}
function Delete(id) {
    $.ajax({
        type: "DELETE",
        url: '/User/Delete',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}
