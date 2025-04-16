var dataTable;

//$(document).ready(function () {
//    loadDataTable();
//});

$(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "ajax": { url: '/user/getall' },
        "columns": [
            {
                data: 'id',
                "render": function (data) {
                    return `<a onClick=QuickDekete('/user/quickdelete?id=${data}') 
                               class="btn btn-sm text-danger" 
                               style="font-size: 1.1rem;">
                                <i class="bi bi-x-circle-fill"></i>
                            </a>`;
                },
                "width": "2%",
                "className": "text-center"
            },
            { data: 'name', "width": "15%" },
            { data: 'email', "width": "10%" },
            { data: 'phoneNumber', "width": "5%" },
            { data: 'company.name', "width": "10%" },
            {
                data: 'role',
                "render": function (data) {
                    // Dynamically add an image based on the role value
                    const iconPath = `/images/icons/${data}.png`;
                    return `
                        <span>
                            <img src="${iconPath}" alt="${data}" style="width: 20px; height: 20px; margin-right: 5px;">
                            ${data}
                        </span>`;
                },
                "width": "10%"
            },
            {
                data: { id: "id", lockoutEnd: "LockoutEnd" },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                        return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:100px;">
                                    <i class="bi bi-lock-fill"></i> lock
                                </a>
                                <a href="/user/RoleManagement?userId=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                    <i class="bi bi-pencil-square"></i> Permission
                                </a>
                            </div>
                            `;
                    }
                    else {
                        return `
                            <div class="text-center">
                                <a onclick=LockUnlock('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width:100px;">
                                    <i class="bi bi-unlock-fill"></i> UnLock
                                </a>
                                <a href="/user/RoleManagement?userId=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                    <i class="bi bi-pencil-square"></i> Permission
                                </a>
                            </div>
                            `;
                    }
                },
                "width": "20%"
            }

        ],
        "columnDefs": [
            { "orderable": false, "targets": [0, 6] }, // Prevent sorting for Status and Action columns
        ]
    });
}

function LockUnlock(id) {
    $.ajax({
        type: "POST",
        url: '/user/LockUnlock',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.warning(data.message);
                dataTable.ajax.reload();
            }
        }
    });
}


function QuickDekete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    // Handle the response here
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}