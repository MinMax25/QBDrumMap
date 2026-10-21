# QBDrumMap language packs

更新者：Codex
最終更新日時：2026-10-19 11:18

In the source project, optional pack files are maintained under `QBDrumMap/LanguagePacks/<culture-code>/`. This source folder is not scanned by the running application. To make a pack available, copy its non-empty `.resx` resource file(s) into `Languages/<culture-code>/` under the application folder.

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
