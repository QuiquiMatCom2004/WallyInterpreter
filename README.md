# WallyInterpreter

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)](https://github.com/USERNAME/WallyInterpreter/actions)  
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Descripción

_WallyInterpreter_ es una aplicación web que ofrece un DSL (Lenguaje de Propósito Específico) para dibujo sobre un canvas responsive. Es ideal para entornos educativos y prototipado rápido de nuevos lenguajes de dibujo.

## Funcionalidades

### Frontend

- Editor de código con resaltado de sintaxis para WallyDSL.  
- Importación y exportación de archivos con extensión `.pw`.  
- Canvas responsive que escala dinámicamente y dibuja píxel a píxel mediante JSInterop.  

### Backend

- Generador de lexer y parser SLR(0) configurables por gramática.  
- Arquitectura modular que permite definir tu propia gramática y ensamblar ASTs fácilmente.  
- Intérprete de WallyDSL extensible para prototipar nuevos comportamientos de dibujo.  

## Tecnologías

- C# con Blazor Server  
- JavaScript y JSInterop  
- MonacoEditor  
- CSS Grid y Flexbox  

## Instalación

1. Clona el repositorio:  
   ```bash
   git clone https://github.com/USERNAME/WallyInterpreter.git
   cd WallyInterpreter
   ```

2. Verifica que tienes .NET SDK 6.0 o superior:  
   ```bash
   dotnet --version
   ```

3. Restaura paquetes y ejecuta la aplicación:  
   ```bash
   dotnet restore
   dotnet run
   ```

4. Abre el navegador en `https://localhost:5001`.

## Uso

1. Escribe tu programa en WallyDSL dentro del editor. Ejemplo mínimo:  
   ```pw
   Spawn(0,0)
   Fill(Color.Red)
   DrawRectangle(5,5,10,8)
   ```

2. Haz clic en **Compile** para interpretar el código y almacenar la matriz.  
3. Serás redirigido a la vista del canvas donde se renderiza tu dibujo.  
4. Usa **Load** y **Save** para manejar archivos `.pw`.  

![Captura de pantalla](docs/screenshot.png)

## Roadmap

- Paletas de colores personalizadas y modos claro/oscuro.  
- Exportación de dibujos a PNG/SVG.  
- Animaciones frame-by-frame.  
- Colaboración en tiempo real con SignalR.  

## Contribuir

1. Haz fork del repositorio.  
2. Crea una rama para tu mejora: `git checkout -b feature-nombre`.  
3. Realiza cambios y haz commit: `git commit -m "Descripción del cambio"`.  
4. Envía pull request describiendo tu aporte.  

## Licencia

Este proyecto está licenciado bajo MIT. Consulta el archivo [LICENSE](LICENSE) para más detalles.

## Contacto

Para dudas, sugerencias o colaboraciones, escríbeme a:  
kikialeglez@gmail.com  
o visita [mi GitHub](https://github.com/USERNAME/WallyInterpreter).
