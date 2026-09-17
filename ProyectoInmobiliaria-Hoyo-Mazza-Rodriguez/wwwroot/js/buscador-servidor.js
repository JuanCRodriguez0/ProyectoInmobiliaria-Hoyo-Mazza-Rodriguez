function inicializarBuscadorServidor(opciones) {
    const input = document.getElementById(opciones.inputId);
    const hidden = document.getElementById(opciones.hiddenId);
    const lista = document.getElementById(opciones.listaId);
    let timeoutId = null;

    function ocultarLista() {
        lista.innerHTML = '';
        lista.style.display = 'none';
    }

    function buscar(texto) {
        if (!texto || texto.length < 2) {
            ocultarLista();
            return;
        }
        fetch(`${opciones.url}?term=${encodeURIComponent(texto)}`)
            .then(r => r.json())
            .then(items => {
                lista.innerHTML = '';
                if (items.length === 0) {
                    ocultarLista();
                    return;
                }
                items.forEach(item => {
                    const boton = document.createElement('button');
                    boton.type = 'button';
                    boton.className = 'list-group-item list-group-item-action';
                    boton.textContent = item.text;
                    boton.addEventListener('click', () => {
                        input.value = item.text;
                        hidden.value = item.id;
                        ocultarLista();
                        if (typeof opciones.onSeleccionar === 'function') {
                            opciones.onSeleccionar(item);
                        }
                    });
                    lista.appendChild(boton);
                });
                lista.style.display = 'block';
            });
    }

    input.addEventListener('input', function () {
        hidden.value = '';
        clearTimeout(timeoutId);
        const texto = this.value;
        timeoutId = setTimeout(() => buscar(texto), 300);
    });

    document.addEventListener('click', function (e) {
        if (e.target !== input && !lista.contains(e.target)) {
            ocultarLista();
        }
    });
}