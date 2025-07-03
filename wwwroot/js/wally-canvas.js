console.log("wally-canvas.js cargado");
window.drawMatrixResponsive = (canvasId, colorArray, rows, cols) => {
    console.log("drawMatrixResponsive:", canvasId, rows, cols);
    const canvas = document.getElementById(canvasId);
    const ctx = canvas.getContext("2d");
    console.log("Elemento canvas:", canvas);

    // Tamaño CSS real del canvas en pantalla
    const displayW = canvas.clientWidth;
    const displayH = canvas.clientHeight;

    // Calcula píxel más grande que cabe entero
    const pixelSize = Math.floor(Math.min(displayW / cols,
        displayH / rows));
    // Ajusta el tamaño de bitmap interno
    canvas.width = cols * pixelSize;
    canvas.height = rows * pixelSize;

    ctx.imageSmoothingEnabled = false;
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Dibuja cada celda como un rectángulo pixelSize×pixelSize
    for (let y = 0; y < rows; y++) {
        for (let x = 0; x < cols; x++) {
            ctx.fillStyle = colorArray[y * cols + x];
            ctx.fillRect(x * pixelSize,
                y * pixelSize,
                pixelSize,
                pixelSize);
        }
    }
};
window.drawMatrix = (canvasId, colorArray, rows, cols, pixelSize) => {
    const canvas = document.getElementById(canvasId);
    const ctx = canvas.getContext("2d");
    // Ajusta tamaño real del canvas
    canvas.width = cols * pixelSize;
    canvas.height = rows * pixelSize;
    ctx.imageSmoothingEnabled = false;

    // Limpia
    ctx.clearRect(0, 0, canvas.width, canvas.height);

    // Dibuja cada “píxel”
    for (let y = 0; y < rows; y++) {
        for (let x = 0; x < cols; x++) {
            const color = colorArray[y * cols + x];
            ctx.fillStyle = color;
            ctx.fillRect(x * pixelSize, y * pixelSize, pixelSize, pixelSize);
        }
    }
};

