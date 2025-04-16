var dataTable;

$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "ajax": { url: '/company/getall' },
        "columns": [
            {
                data: 'id',
                "render": function (data) {
                    return `<a onClick=QuickDekete('/company/quickdelete?id=${data}') 
                               class="btn btn-sm text-danger" 
                               style="font-size: 1.1rem;">
                                <i class="bi bi-x-circle-fill"></i>
                            </a>`;
                },
                "width": "2%",
                "className": "text-center"
            },
            { data: 'name', "width": "10%" },
            { data: 'streetAddress', "width": "20%" },
            { data: 'city', "width": "10%" },
            { data: 'state', "width": "5%" },
            { data: 'phoneNumber', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/company/edit?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                    <a href="/company/delete?id=${data}" class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>

                    </div>`;
                },
                "width": "18%"
            }
        ],
        "columnDefs": [
            { "orderable": false, "targets": [0, 6] }, // Prevent sorting for Status and Action columns
        ],
        "initComplete": function () {
            $('[data-bs-toggle="tooltip"]').tooltip({ html: true }); // Initialize Bootstrap tooltips
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