$(() => {
    $("#new-simcha").on('click', function(){
        new bootstrap.Modal($("#add-simcha")[0]).show();
    })
})