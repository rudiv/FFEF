# FFEF
F*ck Front-End Frameworks

Rather than pull in over 9000 node_modules just to render a basic site, precompile using Razor

## Roadmap

 - Caching
 - Composite View Engines
 - Better Layouting
 - Better Model support
 - ViewBag for loose typeless fun?

## Razor Support

`@model` directive is available to type the model

Setting `Layout` variable defines the parent layout to be used in rendering. Layouts should have `@inherit FFEF.Infrastructure.RazorLayout`, and contain `@async RenderBody()`.

## Usage

Add tool to `PATH`

### VSCode

In your `tasks.json`, where views are in a `Views` folder and to be compiled to root

    {
        "label": "ffef",
        "type": "shell",
        "command": "ffef",
        "args": [
            "${workspaceFolder}\\Views",
            "${workspaceFolder}"
        ],
        "isBackground": true,
        "group": {
            "kind": "build",
            "isDefault": true
        }
    }
