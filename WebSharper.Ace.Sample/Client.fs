namespace WebSharper.Ace.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Notation
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.Ace

[<JavaScript>]
module Client =

    // The template is loaded from the DOM, so editing index.html and refreshing
    // the browser is enough; a rebuild is only needed when the holes change.
    type IndexTemplate = Template<"index.html", ClientLoad.FromDocument>

    let startCode =
        "// Welcome to the WebSharper Ace demo — edit freely!\n" +
        "type Greeting = { Name: string }\n" +
        "\n" +
        "let greet g = sprintf \"Hello, %s!\" g.Name\n"

    // View state, bound into the template.
    let status = Var.Create "Initializing the editor..."
    let cursor = Var.Create "1:1"
    let selection = Var.Create "(no selection)"
    let info = Var.Create "-"
    let gotoLine = Var.Create 1 : Var<int>
    let findText = Var.Create "greet"
    let replaceText = Var.Create "hello"

    // The editor is created on load; handlers below go through `withEditor`.
    let mutable editorOpt : option<Editor> = None

    let withEditor (f: Editor -> unit) =
        match editorOpt with
        | Some editor -> f editor
        | None -> ()

    let refreshCursor () =
        withEditor (fun editor ->
            let pos = editor.GetCursorPosition()
            cursor := sprintf "%d : %d" (pos.Row + 1) (pos.Column + 1)
            let selected = editor.GetSelectedText()
            selection := if selected = "" then "(no selection)" else selected
        )

    let refreshInfo () =
        withEditor (fun editor ->
            let session = editor.Session
            info :=
                sprintf "%d lines · %d characters · undo: %b"
                    (session.GetLength())
                    (session.GetValue().Length)
                    (session.GetUndoManager().HasUndo())
        )

    // ---------------------------------------------------------------
    // Actions driven from the toolbar
    // ---------------------------------------------------------------

    let setMode (mode: string) (label: string) =
        withEditor (fun editor ->
            editor.Session.SetMode mode
            status := "Switched to " + label + "."
        )
        refreshInfo ()

    let setTheme (theme: string) (label: string) =
        withEditor (fun editor ->
            editor.SetTheme theme
            status := "Applied the " + label + " theme."
        )

    let undo () =
        withEditor (fun editor ->
            editor.Undo()
            editor.Focus()
        )
        refreshInfo ()
        refreshCursor ()
        status := "Undid the last change."

    let redo () =
        withEditor (fun editor ->
            editor.Redo()
            editor.Focus()
        )
        refreshInfo ()
        refreshCursor ()
        status := "Redid the last change."

    let focusEditor () =
        withEditor (fun editor -> editor.Focus())

    let goToLine () =
        let line = gotoLine.Value
        withEditor (fun editor ->
            editor.GotoLine(line)
            editor.Focus()
        )
        status := sprintf "Jumped to line %d." line

    let findNext () =
        withEditor (fun editor ->
            let options = SearchOptions()
            options.Wrap <- true
            options.CaseSensitive <- false
            editor.Find(findText.Value, options) |> ignore
            editor.Focus()
        )
        status := "Searched for \"" + findText.Value + "\"."

    let replaceAll () =
        withEditor (fun editor ->
            let options = SearchOptions()
            options.Wrap <- true
            let count = editor.ReplaceAll(replaceText.Value, options)
            status := sprintf "Replaced %d occurrence(s)." count
        )
        refreshInfo ()

    let insertText () =
        withEditor (fun editor ->
            editor.Insert "// inserted at the cursor by the binding\n"
            editor.Focus()
        )
        refreshInfo ()
        status := "Inserted text at the cursor."

    let clearEditor () =
        withEditor (fun editor ->
            editor.SetValue "" |> ignore
            editor.Focus()
        )
        refreshInfo ()
        refreshCursor ()

    let selectAll () =
        withEditor (fun editor ->
            editor.SelectAll()
            editor.Focus()
        )
        refreshCursor ()
        status := "Selected the whole document."

    let addMarker () =
        withEditor (fun editor ->
            // A range spanning the first few characters of line 1.
            let range = Range(0, 0, 0, 5)
            editor.Session.AddMarker(range, "demo-marker", "text") |> ignore
            editor.Session.AddGutterDecoration(0, "demo-gutter") |> ignore
            status := "Added a range marker and a gutter decoration on line 1."
        )

    let addAnnotation () =
        withEditor (fun editor ->
            editor.Session.SetAnnotations [|
                Annotation(2, 0, "An annotation added from F#.", "warning")
            |]
            status := "Added an annotation to line 3."
        )

    let clearDecorations () =
        withEditor (fun editor ->
            editor.Session.ClearAnnotations()
            editor.Session.ClearBreakpoints()
            status := "Cleared annotations and breakpoints."
        )

    let toggleReadOnly () =
        withEditor (fun editor ->
            let next = not (editor.GetReadOnly())
            editor.SetReadOnly(next)
            status := if next then "Editor is read-only." else "Editor is editable."
        )

    let toggleWrap () =
        withEditor (fun editor ->
            let session = editor.Session
            let next = not (session.GetUseWrapMode())
            session.SetUseWrapMode(next)
            status := if next then "Soft wrap enabled." else "Soft wrap disabled."
        )

    let toggleInvisibles () =
        withEditor (fun editor ->
            let next = not (editor.GetShowInvisibles())
            editor.SetShowInvisibles(next)
            status := if next then "Showing invisible characters." else "Hiding invisible characters."
        )

    // ---------------------------------------------------------------
    // A tour of the API surface
    // ---------------------------------------------------------------

    let probeApi () =
        withEditor (fun editor ->
            let session = editor.Session

            // Position / Range round-trip.
            let pos = Position(1, 2)
            let index = session.Doc.PositionToIndex(pos)
            let back = session.Doc.IndexToPosition(index)

            // Range helpers.
            let range = Range(0, 0, 1, 4)
            let empty = range.IsEmpty()
            let multiline = range.IsMultiLine()
            let clipped = range.Clone().ClipRows(0, 1)

            // Anchor.
            let anchor = session.Doc.CreateAnchor(0, 0)
            let anchorPos = anchor.GetPosition()

            // Token at the start of the first row.
            let token = session.GetTokenAt(0, 0)

            // Fold + search helpers.
            let wordRange = session.GetWordRange(0, 0)

            status :=
                sprintf
                    "index(%d,%d)=%d -> (%d,%d) · range empty=%b multiline=%b clip=(%d,%d)-(%d,%d) · anchor=(%d,%d) · token=%s \"%s\" · word=(%d-%d)"
                    pos.Row pos.Column index back.Row back.Column
                    empty multiline
                    clipped.Start.Row clipped.Start.Column clipped.End.Row clipped.End.Column
                    anchorPos.Row anchorPos.Column
                    token.Type token.Value
                    wordRange.Start.Column wordRange.End.Column
        )

    // ---------------------------------------------------------------
    // Entry point
    // ---------------------------------------------------------------

    [<SPAEntryPoint>]
    let Main () =
        // `.Bind()` renders the whole document body, so the template (including
        // the #editor container) is in the DOM only after this call.
        IndexTemplate()
            .status(status.V)
            .cursor(cursor.V)
            .selection(selection.V)
            .info(info.V)
            .gotoLine(gotoLine)
            .findText(findText)
            .replaceText(replaceText)
            .setModeFsharp(fun _ -> setMode "ace/mode/fsharp" "F#")
            .setModeJs(fun _ -> setMode "ace/mode/javascript" "JavaScript")
            .setModeHtml(fun _ -> setMode "ace/mode/html" "HTML")
            .setModeJson(fun _ -> setMode "ace/mode/json" "JSON")
            .setThemeMonokai(fun _ -> setTheme "ace/theme/monokai" "Monokai")
            .setThemeTextmate(fun _ -> setTheme "ace/theme/textmate" "TextMate")
            .setThemeGithub(fun _ -> setTheme "ace/theme/github" "GitHub")
            .undo(fun _ -> undo ())
            .redo(fun _ -> redo ())
            .focusEditor(fun _ -> focusEditor ())
            .gotoLineBtn(fun _ -> goToLine ())
            .findNext(fun _ -> findNext ())
            .replaceAll(fun _ -> replaceAll ())
            .insertText(fun _ -> insertText ())
            .clearEditor(fun _ -> clearEditor ())
            .selectAll(fun _ -> selectAll ())
            .addMarker(fun _ -> addMarker ())
            .addAnnotation(fun _ -> addAnnotation ())
            .clearDecorations(fun _ -> clearDecorations ())
            .toggleReadOnly(fun _ -> toggleReadOnly ())
            .toggleWrap(fun _ -> toggleWrap ())
            .toggleInvisibles(fun _ -> toggleInvisibles ())
            .probeApi(fun _ -> probeApi ())
            .Bind()

        // The template is now in the DOM; create the editor in its container.
        let options = EditorOptions()
        options.Theme <- "ace/theme/monokai"
        options.Mode <- "ace/mode/fsharp"
        options.FontSize <- "14px"
        options.TabSize <- 2
        options.ShowPrintMargin <- false
        options.HighlightActiveLine <- true
        options.UseSoftTabs <- true
        // Ace's background workers (linting/autocomplete) are loaded by URL, not
        // bundled. This sample does not need them, so turn them off; supply
        // `worker-*.js` and set `ace.config` if you want them.
        options.UseWorker <- false

        // `ace.edit` accepts a DOM element or an element **id** (not a CSS
        // selector), so we pass the id of the container.
        let editor = Ace.Edit("editor", options)
        editor.SetValue startCode |> ignore
        editor.ClearSelection()
        editor.Focus()
        editorOpt <- Some editor

        // Live updates: the editor tells us when it changes.
        editor.On("change", fun _ -> refreshInfo ()) |> ignore
        editor.On("changeSelection", fun _ -> refreshCursor ()) |> ignore

        status := "Ace " + Ace.Version + " is ready — start typing."
        refreshInfo ()
        refreshCursor ()
