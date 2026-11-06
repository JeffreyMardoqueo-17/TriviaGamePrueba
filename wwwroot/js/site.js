$(document).on('click', '.arcade-tab, .list-group-item', function(e) {
    var url = $(this).attr('href');
    
    if (url && url !== '#' && !url.startsWith('http')) {
        e.preventDefault(); // Evita la recarga normal de la página
        
        $('.arcade-tab').removeClass('active');
        $(this).addClass('active');

        // Carga el contenido de la vista mediante AJAX en el contenedor principal
        $('.content-area').fadeOut(150, function() {
            $.get(url, function(data) {
                var newContent = $(data).find('.content-area').html();
                if (newContent) {
                    $('.content-area').html(newContent).fadeIn(150);
                } else {
                    $('.content-area').html($(data).find('main').html() || data).fadeIn(150);
                }
                window.history.pushState({}, '', url);
            }).fail(function() {
                window.location.href = url; //POR SI FALLA LA RED
            });
        });
    }
});