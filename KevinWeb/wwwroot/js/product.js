var dataTable;

$(document).ready(function () {
    loadDataTable();
});


function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "ajax": { url: '/product/getall' },
        "columns": [
            {
                data: 'id',
                "render": function (data) {
                    return `<a onClick=QuickDekete('/product/quickdelete?id=${data}') 
                               class="btn btn-sm text-danger" 
                               style="font-size: 1.1rem;">
                                <i class="bi bi-x-circle-fill"></i>
                            </a>`;
                },
                "width": "2%",
                "className": "text-center"
            },
            { data: 'title', "width": "15%" },
            { data: 'isbn', "width": "10%" },
            { data: 'listPrice', "width": "5%" },
            { data: 'author', "width": "10%" },
            { data: 'category.name', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/product/edit?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>
                    <a href="/product/delete?id=${data}" class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>

                    </div>`;
                },
                "width": "20%"
            },
            {
                data: null, // Use `null` to access the entire row
                "render": function (data) {
                    const imageUrl = data.imageUrl?.trim() || "";
                    const productImages = data.productImages || [];

                    if (imageUrl !== "" && productImages.length > 0) {
                        return `<img src="/images/icons/yes-icons.png"
                                     alt="Complete"
                                     width="22"
                                     height="22"
                                     class="status-icon"
                                     data-bs-toggle="tooltip"
                                     data-bs-html="true"
                                     title="<img src='${imageUrl}' alt='Cover Image' width='150' height='150' />" />`;
                    } else {
                        return `<img src="/images/icons/warning-icons.png"
                                     alt="Incomplete"
                                     width="25"
                                     height="25"
                                     class="status-icon"
                                     data-bs-toggle="tooltip"
                                     title="Warning: Product creation is incomplete. Please ensure all images are uploaded." />`;
                    }
                },
                "width": "5%",
                "className": "text-center"
            }

        ],
        "columnDefs": [
            { "orderable": false, "targets": [0, 6, 7] }, // Prevent sorting for Status and Action columns
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