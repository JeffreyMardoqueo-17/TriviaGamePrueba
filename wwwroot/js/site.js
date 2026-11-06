// Función auxiliar para cerrar el offcanvas móvil de forma segura
function closeMobileOffcanvas() {
    var offcanvasElement = document.getElementById('mobileNavbarOffcanvas');
    if (offcanvasElement) {
        // Obtenemos la instancia existente o creamos una nueva
        var bsOffcanvas = bootstrap.Offcanvas.getInstance(offcanvasElement) || new bootstrap.Offcanvas(offcanvasElement);
        if (bsOffcanvas) {
            bsOffcanvas.hide();
        }
    }
}

// 1. Manejo del clic en navegación AJAX
$(document).on('click', '.arcade-tab, .list-group-item', function(e) {
    var $this = $(this);
    var url = $this.attr('href');

    // Cerrar offcanvas siempre que se haga clic en un elemento del menú
    closeMobileOffcanvas();
    
    if (url && url !== '#' && !url.startsWith('http') && !url.startsWith('#')) {
        e.preventDefault();
        
        // Sincronización visual de estados activos
        $('.arcade-tab, .list-group-item').removeClass('active bg-primary text-white shadow-sm').addClass('text-dark bg-light');
        $this.removeClass('text-dark bg-light').addClass('active bg-primary text-white shadow-sm');

        // Transición AJAX del contenido
        $('.content-area').fadeOut(150, function() {
            $.get(url, function(data) {
                var newContent = $(data).find('.content-area').html();
                if (newContent) {
                    $('.content-area').html(newContent).fadeIn(150);
                } else {
                    $('.content-area').html($(data).find('main').html() || data).fadeIn(150);
                }
                window.history.pushState({ path: url }, '', url);
            }).fail(function() {
                window.location.href = url;
            });
        });
    }
});

// 2. Cerrar también al presionar el botón de "Cerrar Sesión" (submit)
$(document).on('submit', '#mobileNavbarOffcanvas form', function() {
    closeMobileOffcanvas();
});

// 3. Manejo de navegación en el historial (atrás / adelante)
$(window).on('popstate', function() {
    if (location.pathname) {
        $.get(location.pathname, function(data) {
            var newContent = $(data).find('.content-area').html();
            if (newContent) {
                $('.content-area').html(newContent);
            } else {
                $('.content-area').html($(data).find('main').html() || data);
            }
        });
    }
});