$(() => {
    $("#new-contributor").on('click', function () {
        ResetModel();
        new bootstrap.Modal($(".new-contrib")[0]).show();
    });

    $(".edit-contrib").on('click', function () {
        const id = $(this).data('id');
        const firstName = $(this).data('first-name');
        const lastName = $(this).data('last-name');
        const cell = $(this).data('cell');
        const alwaysInclude = $(this).data('always-include');
        const date = $(this).data('date');
        $("#contributor_first_name").val(firstName);
        $("#contributor_last_name").val(lastName);
        $("#contributor_cell_number").val(cell); 
        $("#contributor_created_at").val(date);
        if (alwaysInclude == "True") {
            $("#contributor_always_include").prop('checked', true);
        }     
        $(".modal-1-title").text("Edit");
        $("#initialDepositDiv").hide();
        const form = $(".new-contrib form")
        form.attr("action", "/home/edit");
        $("#hiddenId").remove();
        form.append(`<input type="hidden" id="hiddenId" name="id" value="${id}"/>`)
        new bootstrap.Modal($('.new-contrib')[0]).show();
    });

    $(".deposit-button").on('click', function () {
        const id = $(this).data('contribid');
        const name = $(this).data('name')
        $("#contributor-id").val(id);
        $("#deposit-name").text(name);
        new bootstrap.Modal($('.deposit')[0]).show();
    });

    $("#search").on("input", function () {
        var searchText = $(this).val().toLowerCase();
        $(".tr").filter(function () {
            return $(this).text().toLowerCase().indexOf(searchText) === -1;
        }).hide();
        $(".tr").filter(function () {
            return $(this).text().toLowerCase().indexOf(searchText) !== -1;
        }).show();
    });

    $("#clear").on("click", function () {
        $("#search").val("");
        $(".tr").show();
    });

    function ResetModel() {
        $("#contributor_first_name").val("");
        $("#contributor_last_name").val("");
        $("#contributor_cell_number").val("");
        $("#contributor_created_at").val("");
        $("#contributor_always_include").prop('checked', false);
        $(".modal-1-title").text("New Contributor");
        $("#initialDepositDiv").show();
        const form = $(".new-contrib form")
        form.attr("action", "/home/newcontributor");
        $("#hiddenId").remove();
    }
});





