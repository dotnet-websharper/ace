// Ace is consumed from the npm package `ace-builds`, imported as ES modules and
// bundled here. The core module installs the `ace` object; the side-effect
// imports below register the modes and themes this sample uses, so Ace does not
// try to fetch `mode-*.js` / `theme-*.js` from a runtime base path.
import "ace-builds/src-noconflict/ace"
import "ace-builds/src-noconflict/theme-monokai"
import "ace-builds/src-noconflict/theme-github"
import "ace-builds/src-noconflict/theme-textmate"
import "ace-builds/src-noconflict/mode-fsharp"
import "ace-builds/src-noconflict/mode-javascript"
import "ace-builds/src-noconflict/mode-html"
import "ace-builds/src-noconflict/mode-json"

// The WebSharper-generated client bundle (imports the same `ace` module).
import "./build/WebSharper.Ace.Sample.js"
