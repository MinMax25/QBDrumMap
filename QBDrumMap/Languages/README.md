# QBDrumMap language packs

Create one directory per culture code under this directory, for example:

```text
Languages/
  de-DE/
    QBDrumMap.Resources.resx
    QBDrumMap.Name.resx
    de.png
    Manual/
      index.html
```

`QBDrumMap.Resources.resx` contains UI and message strings. `QBDrumMap.Name.resx`
contains domain names such as kit, part, and articulation. Missing keys fall back to
the built-in resources. A pack is discovered at startup when its culture directory
contains at least one non-empty `.resx` file.

The optional flag image uses the lowercase ISO 3166-1 alpha-2 region code from the
culture name, such as `de.png` for `de-DE`.
