function inicializarMapaInmueble(opciones) {
  const mapa = L.map(opciones.mapaId).setView([opciones.lat, opciones.lng], 13);

  L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
    attribution: "&copy; OpenStreetMap contributors",
    maxZoom: 19,
  }).addTo(mapa);

  const inputLat = document.getElementById(opciones.latId);
  const inputLng = document.getElementById(opciones.lngId);

  const marcador = L.marker([opciones.lat, opciones.lng], {
    draggable: true,
  }).addTo(mapa);

  function actualizarInputs(lat, lng) {
    inputLat.value = lat.toFixed(6);
    inputLng.value = lng.toFixed(6);
  }

  if (inputLat.value && inputLng.value) {
    actualizarInputs(parseFloat(inputLat.value), parseFloat(inputLng.value));
  }

  mapa.on("click", function (e) {
    marcador.setLatLng(e.latlng);
    actualizarInputs(e.latlng.lat, e.latlng.lng);
  });

  marcador.on("dragend", function () {
    const pos = marcador.getLatLng();
    actualizarInputs(pos.lat, pos.lng);
  });
}
