// $begin{copyright}
//
// This file is part of WebSharper
//
// Copyright (c) 2008-2018 IntelliFactory
//
// Licensed under the Apache License, Version 2.0 (the "License"); you
// may not use this file except in compliance with the License.  You may
// obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
// implied.  See the License for the specific language governing
// permissions and limitations under the License.
//
// $end{copyright}

// Bindings for the Ace editor (https://ace.c9.io).
//
// The surface modelled here follows the type definitions published with
// `ace-builds` (https://github.com/ajaxorg/ace-builds/tree/master/types),
// specifically the public `Ace` namespace in `ace.d.ts` (Ace 1.45.x): the
// `Document`, `Anchor`, `Range`, `EditSession`, `Selection`, `Editor`,
// `VirtualRenderer`, `UndoManager`, `Tokenizer`, `BackgroundTokenizer`,
// `TokenIterator`, `Search` and `CommandManager` classes, and the `ace`
// module itself.
//
// Where Ace accepts a union (for example `edit(el: string | HTMLElement)`),
// the binding exposes separate overloads instead: the .NET union type WIG
// generates for `A | B` is explicit (`Union1Of2`/`Union2Of2`), which is not
// ergonomic at the call site.

namespace WebSharper.Ace

open WebSharper
open WebSharper.JavaScript
open WebSharper.InterfaceGenerator

[<AutoOpen>]
module Types =

    let String  = T<string>
    let Element = T<Dom.Element>
    let Object  = T<obj>
    let Integer = T<int>
    let Boolean = T<bool>
    let Unit    = T<unit>

module Definition =

    /// Ace delivers its events through a plain `EventEmitter` mixin rather than
    /// exposing it as a global. Instead of generating a bogus .NET type for it,
    /// these members are spliced into every class that can emit events.
    let eventEmitter : unit -> list<CodeModel.IClassMember> =
        fun () ->
            [
                "on" => (String * (Object ^-> Unit)) ^-> Object
                "once" => (String * (Object ^-> Unit)) ^-> Unit
                "off" => String * (Object ^-> Unit) ^-> Unit
                "addEventListener" => String * (Object ^-> Unit) ^-> Unit
                "removeEventListener" => String * (Object ^-> Unit) ^-> Unit
                "removeListener" => String * (Object ^-> Unit) ^-> Unit
                "removeAllListeners" => !?String ^-> Unit
                "setDefaultHandler" => String * (Object ^-> Unit) ^-> Unit
                "removeDefaultHandler" => String * (Object ^-> Unit) ^-> Unit
            ]

    /// Appends the shared event-emitter members to a class instance member list.
    let withEvents (xs: list<CodeModel.IClassMember>) : list<CodeModel.IClassMember> =
        xs @ eventEmitter ()

    // -----------------------------------------------------------------
    // Forward declarations. Every class is created here and extended below,
    // so the classes can reference each other freely.
    // -----------------------------------------------------------------

    // Ace is consumed as the npm package `ace-builds`, imported as an ES
    // module and bundled by the consumer. The `src-noconflict` entry exports
    // the `ace` object (and its classes) without relying on a `<script>` tag.
    let aceModule = "ace-builds/src-noconflict/ace"

    let Document            = Class "ace.Document"
    let Anchor              = Class "ace.Anchor"
    let Range               = Class "ace.Range" |> Import "Range" aceModule
    let EditSession         = Class "ace.EditSession" |> Import "EditSession" aceModule
    let Selection           = Class "ace.Selection"
    let Editor              = Class "ace.Editor" |> Import "Editor" aceModule
    let VirtualRenderer     = Class "ace.VirtualRenderer" |> Import "VirtualRenderer" aceModule
    let UndoManager         = Class "ace.UndoManager" |> Import "UndoManager" aceModule
    let Tokenizer           = Class "ace.Tokenizer"
    let BackgroundTokenizer = Class "ace.BackgroundTokenizer"
    let TokenIterator       = Class "ace.TokenIterator"
    let Search              = Class "ace.Search"
    let CommandManager      = Class "ace.CommandManager"
    let Ace                 = Class "ace" |> WithSourceName "Ace" |> Import "*" aceModule

    // -----------------------------------------------------------------
    // String-union enumerations
    // -----------------------------------------------------------------

    let NewLineMode =
        Pattern.EnumStrings "NewLineMode" [ "windows"; "unix"; "auto" ]

    let SelectionStyle =
        Pattern.EnumStrings "SelectionStyle" [ "fullLine"; "screenLine"; "text"; "line" ]

    let FoldStyle =
        Pattern.EnumStrings "FoldStyle" [ "markbegin"; "markbeginend"; "manual" ]

    let WrapMethod =
        Pattern.EnumStrings "WrapMethod" [ "code"; "text"; "auto" ]

    // -----------------------------------------------------------------
    // Plain object shapes
    // -----------------------------------------------------------------

    /// An Ace `{ row, column }` point, the type Ace calls `Point`.
    let Position =
        Pattern.Config "Position" {
            Required = [ "row", Integer; "column", Integer ]
            Optional = []
        }

    let Delta =
        Pattern.Config "Delta" {
            Required = [
                "action", String
                "start", Position.Type
                "end", Position.Type
                "lines", !|String
            ]
            Optional = [
                "id", Integer
                "folds", !|Object
            ]
        }

    let Token =
        Pattern.Config "Token" {
            Required = [ "type", String; "value", String ]
            Optional = [ "index", Integer; "start", Integer ]
        }

    let Annotation =
        Pattern.Config "Annotation" {
            Required = [ "row", Integer; "column", Integer; "text", String; "type", String ]
            Optional = []
        }

    let SearchOptions =
        Pattern.Config "SearchOptions" {
            Required = []
            Optional = [
                "needle", String
                "preventScroll", Boolean
                "backwards", Boolean
                "start", Position.Type
                "skipCurrent", Boolean
                "range", Range.Type
                "preserveCase", Boolean
                "regExp", Boolean
                "wholeWord", Boolean
                "caseSensitive", Boolean
                "wrap", Boolean
            ]
        }

    let EditorOptions =
        Pattern.Config "EditorOptions" {
            Required = []
            Optional = [
                // Editor behaviour
                "selectionStyle", SelectionStyle.Type
                "highlightActiveLine", Boolean
                "highlightSelectedWord", Boolean
                "readOnly", Boolean
                "copyWithEmptySelection", Boolean
                "cursorStyle", String
                "mergeUndoDeltas", Boolean
                "behavioursEnabled", Boolean
                "wrapBehavioursEnabled", Boolean
                "enableAutoIndent", Boolean
                "autoScrollEditorIntoView", Boolean
                "keyboardHandler", String
                "placeholder", String
                "value", String
                "session", EditSession.Type
                "showFoldWidgets", Boolean
                "fadeFoldWidgets", Boolean
                // Session
                "mode", String
                "newLineMode", NewLineMode.Type
                "useWorker", Boolean
                "useSoftTabs", Boolean
                "tabSize", Integer
                "navigateWithinSoftTabs", Boolean
                "overwrite", Boolean
                "wrap", Boolean
                "wrapMethod", WrapMethod.Type
                "indentedSoftWrap", Boolean
                "firstLineNumber", Integer
                "foldStyle", FoldStyle.Type
                // Renderer
                "animatedScroll", Boolean
                "showInvisibles", Boolean
                "showPrintMargin", Boolean
                "printMarginColumn", Integer
                "showGutter", Boolean
                "showLineNumbers", Boolean
                "displayIndentGuides", Boolean
                "highlightIndentGuides", Boolean
                "highlightGutterLine", Boolean
                "hScrollBarAlwaysVisible", Boolean
                "vScrollBarAlwaysVisible", Boolean
                "fontSize", String
                "fontFamily", String
                "maxLines", Integer
                "minLines", Integer
                "scrollPastEnd", Integer
                "fixedWidthGutter", Boolean
                "theme", String
                "useSvgGutterIcons", Boolean
                "showFoldedAnnotations", Boolean
                "useResizeObserver", Boolean
            ]
        }

    /// The values returned by `ace.config.all()` and accepted by `get`/`set`.
    let ConfigOptions =
        Pattern.Config "ConfigOptions" {
            Required = []
            Optional = [
                "packaged", Boolean
                "workerPath", String
                "modePath", String
                "themePath", String
                "basePath", String
                "suffix", String
                "loadWorkerFromBlob", Boolean
                "sharedPopups", Boolean
                "useStrictCSP", Boolean
            ]
        }

    let Command =
        Pattern.Config "Command" {
            Required = []
            Optional = [
                "name", String
                "bindKey", String
                "readOnly", Boolean
                "exec", (Editor * Object ^-> Unit)
                "isAvailable", (Editor ^-> Boolean)
                "description", String
                "multiSelectAction", String
                "scrollIntoView", String
                "aceCommandGroup", String
                "passEvent", Boolean
                "level", Integer
                "action", String
            ]
        }

    /// The `ace.config` object.
    let AceConfig =
        Class "Config"
        |+> Instance (withEvents [
            "get" => String?key ^-> Object
            "set" => String?key * Object?value ^-> Unit
            "all" => Unit ^-> ConfigOptions
            "moduleUrl" => String?name * !?String?componentName ^-> String
            "setModuleUrl" => String?name * String?subst ^-> String
            "setLoader" => ((String * (Object ^-> Unit)) ^-> Unit) ^-> Unit
            "loadModule" => String?moduleId * (Object ^-> Unit)?onLoad ^-> Unit
            "loadModule" => (!|String)?moduleId * (Object ^-> Unit)?onLoad ^-> Unit
            "defineOptions" => Object?obj * String?path * Object?options ^-> Object
            "setDefaultValue" => String?path * String?name * Object?value ^-> Boolean
            "setDefaultValues" => String?path * Object?optionHash ^-> Unit
            "version" =? String
        ])

    // -----------------------------------------------------------------
    // Document
    // -----------------------------------------------------------------

    let DocumentClass =
        Document
        |+> Instance (withEvents [
            "setValue" => String?text ^-> Unit
            "getValue" => Unit ^-> String
            "createAnchor" => Integer?row * Integer?column ^-> Anchor
            "getNewLineCharacter" => Unit ^-> String
            "setNewLineMode" => NewLineMode.Type?newLineMode ^-> Unit
            "getNewLineMode" => Unit ^-> NewLineMode.Type
            "isNewLine" => String?text ^-> Boolean
            "getLine" => Integer?row ^-> String
            "getLines" => Integer?firstRow * Integer?lastRow ^-> !|String
            "getAllLines" => Unit ^-> !|String
            "getLength" => Unit ^-> Integer
            "getTextRange" => Range?range ^-> String
            "getLinesForRange" => Range?range ^-> !|String
            "insertLines" => Integer?row * (!|String)?lines ^-> Unit
                |> ObsoleteWithMessage "Deprecated by Ace; use insertMergedLines instead."
            "removeLines" => Integer?firstRow * Integer?lastRow ^-> !|String
                |> ObsoleteWithMessage "Deprecated by Ace; use removeFullLines instead."
            "insertNewLine" => Position?position ^-> Position
                |> ObsoleteWithMessage "Deprecated by Ace; use insert instead."
            "insert" => Position?position * String?text ^-> Position
            "insertInLine" => Position?position * String?text ^-> Position
            "clippedPos" => Integer?row * Integer?column ^-> Position
            "clonePos" => Position?pos ^-> Position
            "pos" => Integer?row * Integer?column ^-> Position
            "insertFullLines" => Integer?row * (!|String)?lines ^-> Unit
            "insertMergedLines" => Position?position * (!|String)?lines ^-> Position
            "remove" => Range?range ^-> Position
            "removeInLine" => Integer?row * Integer?startColumn * Integer?endColumn ^-> Position
            "removeFullLines" => Integer?firstRow * Integer?lastRow ^-> !|String
            "removeNewLine" => Integer?row ^-> Unit
            "replace" => Range?range * String?text ^-> Position
            "applyDeltas" => (!|Delta)?deltas ^-> Unit
            "revertDeltas" => (!|Delta)?deltas ^-> Unit
            "applyDelta" => Delta?delta * !?Boolean?doNotValidate ^-> Unit
            "revertDelta" => Delta?delta ^-> Unit
            "indexToPosition" => Integer?index * !?Integer?startRow ^-> Position
            "positionToIndex" => Position?pos * !?Integer?startRow ^-> Integer
        ])

    // -----------------------------------------------------------------
    // Anchor
    // -----------------------------------------------------------------

    let AnchorClass =
        Anchor
        |+> Instance (withEvents [
            "getPosition" => Unit ^-> Position
            "getDocument" => Unit ^-> Document
            "setPosition" => Integer?row * Integer?column * !?Boolean?noClip ^-> Unit
            "detach" => Unit ^-> Unit
            "attach" => Document?doc ^-> Unit
            "row" =? Integer
            "column" =@ Integer
            "document" =? Document
            "markerId" =@ Integer
        ])

    // -----------------------------------------------------------------
    // Range
    // -----------------------------------------------------------------

    let RangeClass =
        Range
        |+> Static [
            Constructor Unit
            Constructor (Integer * Integer * Integer * Integer)
            "fromPoints" => Position?start * Position?stop ^-> Range
            "comparePoints" => Position?p1 * Position?p2 ^-> Integer
        ]
        |+> Instance [
            "start" =@ Position
            "end" =@ Position
            "id" =@ Integer
            "cursor" =@ Position
            "isBackwards" =@ Boolean
            "isEqual" => Range?range ^-> Boolean
            "toString" => Unit ^-> String
            "contains" => Integer?row * Integer?column ^-> Boolean
            "compareRange" => Range?range ^-> Integer
            "comparePoint" => Position?p ^-> Integer
            "containsRange" => Range?range ^-> Boolean
            "intersects" => Range?range ^-> Boolean
            "isEnd" => Integer?row * Integer?column ^-> Boolean
            "isStart" => Integer?row * Integer?column ^-> Boolean
            "setStart" => Integer?row * Integer?column * !?Boolean?noClip ^-> Unit
            "setEnd" => Integer?row * Integer?column * !?Boolean?noClip ^-> Unit
            "inside" => Integer?row * Integer?column ^-> Boolean
            "insideStart" => Integer?row * Integer?column ^-> Boolean
            "insideEnd" => Integer?row * Integer?column ^-> Boolean
            "compare" => Integer?row * Integer?column ^-> Integer
            "compareStart" => Integer?row * Integer?column ^-> Integer
            "compareEnd" => Integer?row * Integer?column ^-> Integer
            "compareInside" => Integer?row * Integer?column ^-> Integer
            "clipRows" => Integer?firstRow * Integer?lastRow ^-> Range
            "extend" => Integer?row * Integer?column ^-> Range
            "isEmpty" => Unit ^-> Boolean
            "isMultiLine" => Unit ^-> Boolean
            "clone" => Unit ^-> Range
            "collapseRows" => Unit ^-> Range
            "toScreenRange" => EditSession?session ^-> Range
            "moveBy" => Integer?row * Integer?column ^-> Unit
        ]

    // -----------------------------------------------------------------
    // EditSession
    // -----------------------------------------------------------------

    let EditSessionClass =
        EditSession
        |+> Static [
            Constructor (String?text * !?String?mode)
            Constructor (Document?text * !?String?mode)
            "fromJSON" => String?session ^-> EditSession
            "fromJSON" => Object?session ^-> EditSession
        ]
        |+> Instance (withEvents [
            "doc" =? Document
            "id" =? String
            "bgTokenizer" =? BackgroundTokenizer
            "selection" =? Selection
            "destroyed" =? Boolean
            "mergeUndoDeltas" =@ Boolean
            "lineWidgets" =? !|Object
            "gutterRenderer" =? Object
            "startOperation" => !?Object?commandEvent ^-> Unit
            "endOperation" => !?Object?e ^-> Unit
            "setDocument" => Document?doc ^-> Unit
            "getDocument" => Unit ^-> Document
            "resetCaches" => Unit ^-> Unit
            "onSelectionChange" => Unit ^-> Unit
            "setValue" => String?text ^-> Unit
            "getValue" => Unit ^-> String
            "toJSON" => Unit ^-> Object
            "getSelection" => Unit ^-> Selection
            "getState" => Integer?row ^-> Object
            "getTokens" => Integer?row ^-> !|Token
            "getTokenAt" => Integer?row * Integer?column ^-> Token
            "setUndoManager" => UndoManager?undoManager ^-> Unit
            "getUndoManager" => Unit ^-> UndoManager
            "markUndoGroup" => Unit ^-> Unit
            "getTabString" => Unit ^-> String
            "setUseSoftTabs" => Boolean?useSoftTabs ^-> Unit
            "getUseSoftTabs" => Unit ^-> Boolean
            "setTabSize" => Integer?tabSize ^-> Unit
            "getTabSize" => Unit ^-> Integer
            "isTabStop" => Position?position ^-> Boolean
            "setNavigateWithinSoftTabs" => Boolean?navigateWithinSoftTabs ^-> Unit
            "getNavigateWithinSoftTabs" => Unit ^-> Boolean
            "setOverwrite" => Boolean?overwrite ^-> Unit
            "getOverwrite" => Unit ^-> Boolean
            "toggleOverwrite" => Unit ^-> Unit
            "addGutterDecoration" => Integer?row * String?className ^-> Unit
            "removeGutterDecoration" => Integer?row * String?className ^-> Unit
            "addGutterCustomWidget" => Integer?row * Object?attributes ^-> Unit
            "removeGutterCustomWidget" => Integer?row ^-> Unit
            "getBreakpoints" => Unit ^-> !|String
            "setBreakpoints" => (!|Integer)?rows ^-> Unit
            "clearBreakpoints" => Unit ^-> Unit
            "setBreakpoint" => Integer?row * String?className ^-> Unit
            "clearBreakpoint" => Integer?row ^-> Unit
            "addMarker" => Range?range * String?clazz * String?markerType * !?Boolean?inFront ^-> Integer
            "addMarker" => Range?range * String?clazz * (Unit ^-> Unit)?renderer * !?Boolean?inFront ^-> Integer
            "addDynamicMarker" => Object?marker * !?Boolean?inFront ^-> Object
            "removeMarker" => Integer?markerId ^-> Unit
            "getMarkers" => !?Boolean?inFront ^-> Object
            "highlight" => T<RegExp>?re ^-> Unit
            "highlightLines" => Integer?startRow * Integer?endRow * String?clazz * !?Boolean?inFront ^-> Range
            "setAnnotations" => (!|Annotation)?annotations ^-> Unit
            "getAnnotations" => Unit ^-> !|Annotation
            "clearAnnotations" => Unit ^-> Unit
            "getWordRange" => Integer?row * Integer?column ^-> Range
            "getAWordRange" => Integer?row * Integer?column ^-> Range
            "setNewLineMode" => NewLineMode.Type?newLineMode ^-> Unit
            "getNewLineMode" => Unit ^-> NewLineMode.Type
            "setUseWorker" => Boolean?useWorker ^-> Unit
            "getUseWorker" => Unit ^-> Boolean
            "setMode" => String?mode * !?(Unit ^-> Unit)?cb ^-> Unit
            "getMode" => Unit ^-> Object
            "setScrollTop" => Integer?scrollTop ^-> Unit
            "getScrollTop" => Unit ^-> Integer
            "setScrollLeft" => Integer?scrollLeft ^-> Unit
            "getScrollLeft" => Unit ^-> Integer
            "getScreenWidth" => Unit ^-> Integer
            "getLineWidgetMaxWidth" => Unit ^-> Integer
            "getLine" => Integer?row ^-> String
            "getLines" => Integer?firstRow * Integer?lastRow ^-> !|String
            "getLength" => Unit ^-> Integer
            "getTextRange" => !?Range?range ^-> String
            "insert" => Position?position * String?text ^-> Position
            "remove" => Range?range ^-> Position
            "removeFullLines" => Integer?firstRow * Integer?lastRow ^-> !|String
            "undoChanges" => (!|Delta)?deltas * !?Boolean?dontSelect ^-> Unit
            "redoChanges" => (!|Delta)?deltas * !?Boolean?dontSelect ^-> Unit
            "setUndoSelect" => Boolean?enable ^-> Unit
            "replace" => Range?range * String?text ^-> Position
            "moveText" => Range?fromRange * Position?toPosition * !?Boolean?copy ^-> Range
            "indentRows" => Integer?startRow * Integer?endRow * String?indentString ^-> Unit
            "outdentRows" => Range?range ^-> Unit
            "moveLinesUp" => Integer?firstRow * Integer?lastRow ^-> Integer
            "moveLinesDown" => Integer?firstRow * Integer?lastRow ^-> Integer
            "duplicateLines" => Integer?firstRow * Integer?lastRow ^-> Integer
            "setUseWrapMode" => Boolean?useWrapMode ^-> Unit
            "getUseWrapMode" => Unit ^-> Boolean
            "setWrapLimitRange" => Integer?min * Integer?max ^-> Unit
            "adjustWrapLimit" => Integer?desiredLimit * !?Object?printMargin ^-> Boolean
            "getWrapLimit" => Unit ^-> Integer
            "setWrapLimit" => Integer?limit ^-> Unit
            "getWrapLimitRange" => Unit ^-> Object
            "getRowLength" => Integer?row ^-> Integer
            "getRowLineCount" => Integer?row ^-> Integer
            "getRowWrapIndent" => Integer?screenRow ^-> Integer
            "getScreenLastRowColumn" => Integer?screenRow ^-> Integer
            "getDocumentLastRowColumn" => Integer?docRow * Integer?docColumn ^-> Integer
            "getDocumentLastRowColumnPosition" => Integer?docRow * Integer?docColumn ^-> Position
            "getRowSplitData" => Integer?row ^-> String
            "getScreenTabSize" => Integer?screenColumn ^-> Integer
            "screenToDocumentRow" => Integer?screenRow * Integer?screenColumn ^-> Integer
            "screenToDocumentColumn" => Integer?screenRow * Integer?screenColumn ^-> Integer
            "screenToDocumentPosition" => Integer?screenRow * Integer?screenColumn ^-> Position
            "documentToScreenPosition" => Integer?docRow * Integer?docColumn ^-> Position
            "documentToScreenColumn" => Integer?row * !?Integer?docColumn ^-> Integer
            "documentToScreenRow" => Integer?docRow * !?Integer?docColumn ^-> Integer
            "getScreenLength" => Unit ^-> Integer
            "getPrecedingCharacter" => Unit ^-> String
            "getSelectionMarkers" => Unit ^-> !|Object
            "destroy" => Unit ^-> Unit
        ])

    // -----------------------------------------------------------------
    // UndoManager
    // -----------------------------------------------------------------

    let UndoManagerClass =
        UndoManager
        |+> Static [
            Constructor Unit
        ]
        |+> Instance [
            "addSession" => EditSession?session ^-> Unit
            "add" => Delta?delta * Boolean?allowMerge * !?EditSession?session ^-> Unit
            "addSelection" => Object?selection * !?Integer?rev ^-> Unit
            "startNewGroup" => Unit ^-> Object
            "markIgnored" => Integer?fromRev * !?Integer?toRev ^-> Unit
            "getSelection" => Integer?rev * !?Boolean?after ^-> Object
            "getRevision" => Unit ^-> Integer
            "getDeltas" => Integer?fromRev * !?Integer?toRev ^-> !|Delta
            "getChangedRanges" => Integer?fromRev * !?Integer?toRev ^-> Unit
            "getChangedLines" => Integer?fromRev * !?Integer?toRev ^-> Unit
            "undo" => EditSession?session * !?Boolean?dontSelect ^-> Unit
            "redo" => EditSession?session * !?Boolean?dontSelect ^-> Unit
            "reset" => Unit ^-> Unit
            "canUndo" => Unit ^-> Boolean
            "canRedo" => Unit ^-> Boolean
            "hasUndo" => Unit ^-> Boolean
            "hasRedo" => Unit ^-> Boolean
            "isClean" => Unit ^-> Boolean
            "bookmark" => !?Integer?rev ^-> Unit
            "isAtBookmark" => Unit ^-> Boolean
            "markClean" => !?Integer?rev ^-> Unit
            "toJSON" => Unit ^-> Object
            "fromJSON" => Object?json ^-> Unit
        ]

    // -----------------------------------------------------------------
    // Selection
    // -----------------------------------------------------------------

    let SelectionClass =
        Selection
        |+> Instance (withEvents [
            "session" =? EditSession
            "doc" =? Document
            "cursor" =? Anchor
            "lead" =? Anchor
            "anchor" =? Anchor
            "isEmpty" => Unit ^-> Boolean
            "isMultiLine" => Unit ^-> Boolean
            "getCursor" => Unit ^-> Position
            "setAnchor" => Integer?row * Integer?column ^-> Unit
            "getAnchor" => Unit ^-> Position
            "getSelectionLead" => Unit ^-> Position
            "isBackwards" => Unit ^-> Boolean
            "getRange" => Unit ^-> Range
            "clearSelection" => Unit ^-> Unit
            "selectAll" => Unit ^-> Unit
            "setRange" => Range?range * !?Boolean?reverse ^-> Unit
            "selectTo" => Integer?row * Integer?column ^-> Unit
            "selectToPosition" => Position?pos ^-> Unit
            "moveTo" => Integer?row * Integer?column ^-> Unit
            "moveToPosition" => Position?pos ^-> Unit
            "selectUp" => Unit ^-> Unit
            "selectDown" => Unit ^-> Unit
            "selectRight" => Unit ^-> Unit
            "selectLeft" => Unit ^-> Unit
            "selectLineStart" => Unit ^-> Unit
            "selectLineEnd" => Unit ^-> Unit
            "selectFileEnd" => Unit ^-> Unit
            "selectFileStart" => Unit ^-> Unit
            "selectWordRight" => Unit ^-> Unit
            "selectWordLeft" => Unit ^-> Unit
            "getWordRange" => Integer?row * Integer?column ^-> Range
            "selectWord" => Unit ^-> Unit
            "selectAWord" => Unit ^-> Unit
            "getLineRange" => Integer?row * Boolean?excludeLastChar ^-> Range
            "selectLine" => Unit ^-> Unit
            "moveCursorUp" => Unit ^-> Unit
            "moveCursorDown" => Unit ^-> Unit
            "wouldMoveIntoSoftTab" => Position?cursor * Integer?tabSize * Integer?direction ^-> Boolean
            "moveCursorLeft" => Unit ^-> Unit
            "moveCursorRight" => Unit ^-> Unit
            "moveCursorLineStart" => Unit ^-> Unit
            "moveCursorLineEnd" => Unit ^-> Unit
            "moveCursorFileEnd" => Unit ^-> Unit
            "moveCursorFileStart" => Unit ^-> Unit
            "moveCursorLongWordRight" => Unit ^-> Unit
            "moveCursorLongWordLeft" => Unit ^-> Unit
            "moveCursorShortWordRight" => Unit ^-> Unit
            "moveCursorShortWordLeft" => Unit ^-> Unit
            "moveCursorWordRight" => Unit ^-> Unit
            "moveCursorWordLeft" => Unit ^-> Unit
            "moveCursorBy" => Integer?rows * Integer?chars ^-> Unit
            "moveCursorToPosition" => Position?position ^-> Unit
            "moveCursorTo" => Integer?row * Integer?column * !?Boolean?keepDesiredColumn ^-> Unit
            "moveCursorToScreen" => Integer?row * Integer?column * Boolean?keepDesiredColumn ^-> Unit
            "detach" => Unit ^-> Unit
            "fromOrientedRange" => Range?range ^-> Unit
            "toOrientedRange" => !?Range?range ^-> Range
            "getRangeOfMovements" => (Unit ^-> Unit)?func ^-> Range
            "toJSON" => Unit ^-> Range
            "fromJSON" => Object?data ^-> Unit
            "isEqual" => Object?data ^-> Boolean
            "setSelectionAnchor" => Integer?row * Integer?column ^-> Unit
            "getSelectionAnchor" => Unit ^-> Position
            "setSelectionRange" => Range?range * !?Boolean?reverse ^-> Unit
            // Multi-select
            "addRange" => Range?range * !?Boolean?blockChangeEvents ^-> Unit
            "getAllRanges" => Unit ^-> !|Range
            "mergeOverlappingRanges" => Unit ^-> Unit
            "splitIntoLines" => Unit ^-> Unit
            "joinSelections" => Unit ^-> Unit
            "toggleBlockSelection" => Unit ^-> Unit
            "substractPoint" => Position?pos ^-> Unit
            "rectangularRangeBlock" => Position?screenCursor * Position?screenAnchor * !?Boolean?includeEmptyLines ^-> !|Range
            "toSingleRange" => !?Range?range ^-> Unit
            "inMultiSelectMode" =@ Boolean
            "rangeCount" =? Integer
        ])

    // -----------------------------------------------------------------
    // VirtualRenderer
    // -----------------------------------------------------------------

    let VirtualRendererClass =
        VirtualRenderer
        |+> Static [
            Constructor (Element * !?String?theme)
            Constructor (!?String?theme)
        ]
        |+> Instance (withEvents [
            "container" =? Element
            "scroller" =? Element
            "content" =? Element
            "canvas" =? Element
            "scrollTop" =@ Integer
            "scrollLeft" =@ Integer
            "cursorPos" =? Position
            "updateCharacterSize" => Unit ^-> Unit
            "characterWidth" =? Integer
            "lineHeight" =? Integer
            "setSession" => EditSession?session ^-> Unit
            "session" =? EditSession
            "updateLines" => Integer?firstRow * Integer?lastRow * !?Boolean?force ^-> Unit
            "updateText" => Unit ^-> Unit
            "updateFull" => !?Boolean?force ^-> Unit
            "updateFontSize" => Unit ^-> Unit
            "adjustWrapLimit" => Unit ^-> Boolean
            "setAnimatedScroll" => Boolean?shouldAnimate ^-> Unit
            "getAnimatedScroll" => Unit ^-> Boolean
            "setShowInvisibles" => Boolean?showInvisibles ^-> Unit
            "getShowInvisibles" => Unit ^-> Boolean
            "getDisplayIndentGuides" => Unit ^-> Boolean
            "setDisplayIndentGuides" => Boolean?display ^-> Unit
            "getHighlightIndentGuides" => Unit ^-> Boolean
            "setHighlightIndentGuides" => Boolean?highlight ^-> Unit
            "setShowPrintMargin" => Boolean?showPrintMargin ^-> Unit
            "getShowPrintMargin" => Unit ^-> Boolean
            "setPrintMarginColumn" => Integer?printMarginColumn ^-> Unit
            "getPrintMarginColumn" => Unit ^-> Integer
            "getShowGutter" => Unit ^-> Boolean
            "setShowGutter" => Boolean?show ^-> Unit
            "getFadeFoldWidgets" => Unit ^-> Boolean
            "setFadeFoldWidgets" => Boolean?show ^-> Unit
            "setHighlightGutterLine" => Boolean?shouldHighlight ^-> Unit
            "getHighlightGutterLine" => Unit ^-> Boolean
            "getContainerElement" => Unit ^-> Element
            "getMouseEventTarget" => Unit ^-> Element
            "getTextAreaContainer" => Unit ^-> Element
            "getFirstVisibleRow" => Unit ^-> Integer
            "getFirstFullyVisibleRow" => Unit ^-> Integer
            "getLastFullyVisibleRow" => Unit ^-> Integer
            "getLastVisibleRow" => Unit ^-> Integer
            "setPadding" => Integer?padding ^-> Unit
            "setScrollMargin" => Integer?top * Integer?bottom * Integer?left * Integer?right ^-> Unit
            "setMargin" => Integer?top * Integer?bottom * Integer?left * Integer?right ^-> Unit
            "getHScrollBarAlwaysVisible" => Unit ^-> Boolean
            "setHScrollBarAlwaysVisible" => Boolean?alwaysVisible ^-> Unit
            "getVScrollBarAlwaysVisible" => Unit ^-> Boolean
            "setVScrollBarAlwaysVisible" => Boolean?alwaysVisible ^-> Unit
            "freeze" => Unit ^-> Unit
            "unfreeze" => Unit ^-> Unit
            "updateFrontMarkers" => Unit ^-> Unit
            "updateBackMarkers" => Unit ^-> Unit
            "addGutterDecoration" => Integer?row * String?className ^-> Unit
            "removeGutterDecoration" => Integer?row * String?className ^-> Unit
            "updateBreakpoints" => !?Object?rows ^-> Unit
            "setAnnotations" => (!|Annotation)?annotations ^-> Unit
            "updateCursor" => Unit ^-> Unit
            "hideCursor" => Unit ^-> Unit
            "showCursor" => Unit ^-> Unit
            "scrollSelectionIntoView" => Position?anchor * Position?lead * !?Integer?offset ^-> Unit
            "scrollCursorIntoView" => !?Position?cursor * !?Integer?offset * !?Object?viewMargin ^-> Unit
            "getScrollTop" => Unit ^-> Integer
            "getScrollLeft" => Unit ^-> Integer
            "getScrollTopRow" => Unit ^-> Integer
            "getScrollBottomRow" => Unit ^-> Integer
            "scrollToRow" => Integer?row ^-> Unit
            "alignCursor" => Position?cursor * !?Integer?alignment ^-> Integer
            "scrollToLine" => Integer?line * Boolean?center * Boolean?animate * !?(Unit ^-> Unit)?callback ^-> Unit
            "animateScrolling" => Integer?fromValue * !?(Unit ^-> Unit)?callback ^-> Unit
            "scrollToY" => Integer?scrollTop ^-> Unit
            "scrollToX" => Integer?scrollLeft ^-> Unit
            "scrollTo" => Integer?x * Integer?y ^-> Unit
            "scrollBy" => Integer?deltaX * Integer?deltaY ^-> Unit
            "isScrollableBy" => Integer?deltaX * Integer?deltaY ^-> Boolean
            "pixelToScreenCoordinates" => Integer?x * Integer?y ^-> Position
            "screenToTextCoordinates" => Integer?x * Integer?y ^-> Position
            "textToScreenCoordinates" => Integer?row * Integer?column ^-> Object
            "visualizeFocus" => Unit ^-> Unit
            "visualizeBlur" => Unit ^-> Unit
            "setCompositionText" => String?text ^-> Unit
            "hideComposition" => Unit ^-> Unit
            "setGhostText" => String?text * !?Position?position ^-> Unit
            "removeGhostText" => Unit ^-> Unit
            "setTheme" => !?String?theme * !?(Unit ^-> Unit)?cb ^-> Unit
            "getTheme" => Unit ^-> String
            "setStyle" => String?style * !?Boolean?includeIt ^-> Unit
            "unsetStyle" => String?style ^-> Unit
            "setCursorStyle" => String?style ^-> Unit
            "setMouseCursor" => String?cursorStyle ^-> Unit
            "attachToShadowRoot" => Unit ^-> Unit
            "destroy" => Unit ^-> Unit
        ])

    // -----------------------------------------------------------------
    // Editor
    // -----------------------------------------------------------------

    let EditorClass =
        Editor
        |+> Static [
            Constructor (VirtualRenderer * !?EditSession * !?EditorOptions)
            Constructor (!?EditSession * !?EditorOptions)
        ]
        |+> Instance (withEvents [
            "id" =? String
            "container" =? Element
            "renderer" =? VirtualRenderer
            "commands" =? CommandManager
            "keyBinding" =? Object
            "textInput" =? Object
            "selection" =? Selection
            "session" =@ EditSession
            "inVirtualSelectionMode" =@ Boolean
            "destroyed" =? Boolean
            "getSession" => Unit ^-> EditSession
            "setSession" => !?EditSession?session ^-> Unit
            "setValue" => String?text * !?Integer?cursorPos ^-> String
            "getValue" => Unit ^-> String
            "getSelection" => Unit ^-> Selection
            "getSelectionRange" => Unit ^-> Range
            "resize" => !?Boolean?force ^-> Unit
            "setTheme" => String?theme * !?(Unit ^-> Unit)?cb ^-> Unit
            "getTheme" => Unit ^-> String
            "setStyle" => String?style * !?Boolean?includeIt ^-> Unit
            "unsetStyle" => String?style ^-> Unit
            "getFontSize" => Unit ^-> String
            "setFontSize" => String?size ^-> Unit
            "setFontSize" => Integer?size ^-> Unit
            "focus" => Unit ^-> Unit
            "isFocused" => Unit ^-> Boolean
            "blur" => Unit ^-> Unit
            "getCopyText" => Unit ^-> String
            "execCommand" => String?command * !?Object?args ^-> Boolean
            "execCommand" => (Command)?command * !?Object?args ^-> Boolean
            "insert" => String?text * !?Boolean?pasted ^-> Unit
            "autoIndent" => Unit ^-> Unit
            "setKeyboardHandler" => String?keyboardHandler * !?(Unit ^-> Unit)?cb ^-> Unit
            "getKeyboardHandler" => Unit ^-> Object
            "setOverwrite" => Boolean?overwrite ^-> Unit
            "getOverwrite" => Unit ^-> Boolean
            "toggleOverwrite" => Unit ^-> Unit
            "setScrollSpeed" => Integer?speed ^-> Unit
            "getScrollSpeed" => Unit ^-> Integer
            "setDragDelay" => Integer?dragDelay ^-> Unit
            "getDragDelay" => Unit ^-> Integer
            "setSelectionStyle" => SelectionStyle.Type?style ^-> Unit
            "getSelectionStyle" => Unit ^-> SelectionStyle.Type
            "setHighlightActiveLine" => Boolean?shouldHighlight ^-> Unit
            "getHighlightActiveLine" => Unit ^-> Boolean
            "setHighlightGutterLine" => Boolean?shouldHighlight ^-> Unit
            "getHighlightGutterLine" => Unit ^-> Boolean
            "setHighlightSelectedWord" => Boolean?shouldHighlight ^-> Unit
            "getHighlightSelectedWord" => Unit ^-> Boolean
            "setAnimatedScroll" => Boolean?shouldAnimate ^-> Unit
            "getAnimatedScroll" => Unit ^-> Boolean
            "setShowInvisibles" => Boolean?showInvisibles ^-> Unit
            "getShowInvisibles" => Unit ^-> Boolean
            "setDisplayIndentGuides" => Boolean?display ^-> Unit
            "getDisplayIndentGuides" => Unit ^-> Boolean
            "setHighlightIndentGuides" => Boolean?highlight ^-> Unit
            "getHighlightIndentGuides" => Unit ^-> Boolean
            "setShowPrintMargin" => Boolean?showPrintMargin ^-> Unit
            "getShowPrintMargin" => Unit ^-> Boolean
            "setPrintMarginColumn" => Integer?showPrintMargin ^-> Unit
            "getPrintMarginColumn" => Unit ^-> Integer
            "setReadOnly" => Boolean?readOnly ^-> Unit
            "getReadOnly" => Unit ^-> Boolean
            "setBehavioursEnabled" => Boolean?enabled ^-> Unit
            "getBehavioursEnabled" => Unit ^-> Boolean
            "setWrapBehavioursEnabled" => Boolean?enabled ^-> Unit
            "getWrapBehavioursEnabled" => Unit ^-> Boolean
            "setShowFoldWidgets" => Boolean?show ^-> Unit
            "getShowFoldWidgets" => Unit ^-> Boolean
            "setFadeFoldWidgets" => Boolean?fade ^-> Unit
            "getFadeFoldWidgets" => Unit ^-> Boolean
            "setOptions" => EditorOptions?options ^-> Unit
            "getOptions" => !?(!|String)?optionNames ^-> EditorOptions
            "setOption" => String?name * Object?value ^-> Unit
            "getOption" => String?name ^-> Object
            "remove" => !?String?dir ^-> Unit
            "removeWordRight" => Unit ^-> Unit
            "removeWordLeft" => Unit ^-> Unit
            "removeToLineStart" => Unit ^-> Unit
            "removeToLineEnd" => Unit ^-> Unit
            "removeLines" => Unit ^-> Unit
            "splitLine" => Unit ^-> Unit
            "transposeLetters" => Unit ^-> Unit
            "toLowerCase" => Unit ^-> Unit
            "toUpperCase" => Unit ^-> Unit
            "indent" => Unit ^-> Unit
            "blockIndent" => Unit ^-> Unit
            "blockOutdent" => Unit ^-> Unit
            "sortLines" => Unit ^-> Unit
            "toggleCommentLines" => Unit ^-> Unit
            "toggleBlockComment" => Unit ^-> Unit
            "toggleWord" => Unit ^-> Unit
            "duplicateSelection" => Unit ^-> Unit
            "moveLinesDown" => Unit ^-> Unit
            "moveLinesUp" => Unit ^-> Unit
            "moveText" => Range?range * Position?toPosition * !?Boolean?copy ^-> Range
            "copyLinesUp" => Unit ^-> Unit
            "copyLinesDown" => Unit ^-> Unit
            "getFirstVisibleRow" => Unit ^-> Integer
            "getLastVisibleRow" => Unit ^-> Integer
            "isRowVisible" => Integer?row ^-> Boolean
            "isRowFullyVisible" => Integer?row ^-> Boolean
            "selectPageDown" => Unit ^-> Unit
            "selectPageUp" => Unit ^-> Unit
            "gotoPageDown" => Unit ^-> Unit
            "gotoPageUp" => Unit ^-> Unit
            "scrollPageDown" => Unit ^-> Unit
            "scrollPageUp" => Unit ^-> Unit
            "scrollToRow" => Integer?row ^-> Unit
            "scrollToLine" => Integer?line * Boolean?center * Boolean?animate * !?(Unit ^-> Unit)?callback ^-> Unit
            "centerSelection" => Unit ^-> Unit
            "getCursorPosition" => Unit ^-> Position
            "getCursorPositionScreen" => Unit ^-> Position
            "selectAll" => Unit ^-> Unit
            "clearSelection" => Unit ^-> Unit
            "moveCursorTo" => Integer?row * Integer?column ^-> Unit
            "moveCursorToPosition" => Position?position ^-> Unit
            "jumpToMatching" => !?Boolean?doSelect * !?Boolean?expand ^-> Unit
            "gotoLine" => Integer?lineNumber * !?Integer?column * !?Boolean?animate ^-> Unit
            "navigateTo" => Integer?row * Integer?column ^-> Unit
            "navigateUp" => !?Integer?times ^-> Unit
            "navigateDown" => !?Integer?times ^-> Unit
            "navigateLeft" => !?Integer?times ^-> Unit
            "navigateRight" => !?Integer?times ^-> Unit
            "navigateLineStart" => Unit ^-> Unit
            "navigateLineEnd" => Unit ^-> Unit
            "navigateFileEnd" => Unit ^-> Unit
            "navigateFileStart" => Unit ^-> Unit
            "navigateWordRight" => Unit ^-> Unit
            "navigateWordLeft" => Unit ^-> Unit
            "replace" => !?String?replacement * !?SearchOptions?options ^-> Integer
            "replaceAll" => !?String?replacement * !?SearchOptions?options ^-> Integer
            "getLastSearchOptions" => Unit ^-> SearchOptions
            "find" => String?needle * !?SearchOptions?options * !?Boolean?animate ^-> Range
            "find" => T<RegExp>?needle * !?SearchOptions?options * !?Boolean?animate ^-> Range
            "findNext" => !?SearchOptions?options * !?Boolean?animate ^-> Unit
            "findPrevious" => !?SearchOptions?options * !?Boolean?animate ^-> Unit
            "revealRange" => Range?range * !?Boolean?animate ^-> Unit
            "undo" => Unit ^-> Unit
            "redo" => Unit ^-> Unit
            "destroy" => Unit ^-> Unit
            "setAutoScrollEditorIntoView" => Boolean?enable ^-> Unit
            "setGhostText" => String?text * !?Position?position ^-> Unit
            "removeGhostText" => Unit ^-> Unit
            // Multi-select
            "addSelectionMarker" => Range?orientedRange ^-> Range
            "removeSelectionMarker" => Range?range ^-> Unit
            "removeSelectionMarkers" => (!|Range)?ranges ^-> Unit
            "forEachSelection" => Object?cmd * !?String?args * !?Object?options ^-> Unit
            "exitMultiSelectMode" => Unit ^-> Unit
            "getSelectedText" => Unit ^-> String
            "findAll" => !?String?needle * !?SearchOptions?options * !?Boolean?additive ^-> Integer
            "selectMoreLines" => Integer?dir * !?Boolean?skip ^-> Unit
            "transposeSelections" => Integer?dir ^-> Unit
            "selectMore" => Integer?dir * !?Boolean?skip * !?Boolean?stopAtFirst ^-> Unit
            "alignCursors" => Unit ^-> Unit
            "updateSelectionMarkers" => Unit ^-> Unit
        ])

    // -----------------------------------------------------------------
    // Tokenizer / BackgroundTokenizer / TokenIterator
    // -----------------------------------------------------------------

    let TokenizerClass =
        Tokenizer
        |+> Instance [
            "splitRegex" =? T<RegExp>
            "states" =? Object
            "removeCapturingGroups" => String?src ^-> String
            "createSplitterRegexp" => String?src * String?flag ^-> T<RegExp>
            "getLineTokens" => String?line * (!|String)?startState ^-> Object
            "getLineTokens" => String?line * String?startState ^-> Object
        ]

    let BackgroundTokenizerClass =
        BackgroundTokenizer
        |+> Instance (withEvents [
            "lines" =? Object
            "states" =? Object
            "currentLine" =? Integer
            "tokenizer" =? Tokenizer
            "setTokenizer" => Tokenizer?tokenizer ^-> Unit
            "setDocument" => Document?doc ^-> Unit
            "doc" =? Document
            "fireUpdateEvent" => Integer?firstRow * Integer?lastRow ^-> Unit
            "start" => Integer?startRow ^-> Unit
            "scheduleStart" => Unit ^-> Unit
            "stop" => Unit ^-> Unit
            "getTokens" => Integer?row ^-> !|Token
            "getState" => Integer?row ^-> Object
            "cleanup" => Unit ^-> Unit
        ])

    let TokenIteratorClass =
        TokenIterator
        |+> Instance [
            "stepBackward" => Unit ^-> Token
            "stepForward" => Unit ^-> Token
            "getCurrentToken" => Unit ^-> Token
            "getCurrentTokenRow" => Unit ^-> Integer
            "getCurrentTokenColumn" => Unit ^-> Integer
            "getCurrentTokenPosition" => Unit ^-> Position
            "getCurrentTokenRange" => Unit ^-> Range
        ]

    // -----------------------------------------------------------------
    // Search
    // -----------------------------------------------------------------

    let SearchClass =
        Search
        |+> Instance [
            "set" => SearchOptions?options ^-> Search
            "getOptions" => Unit ^-> SearchOptions
            "setOptions" => SearchOptions?options ^-> Unit
            "find" => EditSession?session ^-> Range
            "findAll" => EditSession?session ^-> !|Range
            "parseReplaceString" => String?replaceString ^-> Object
            "replace" => String?input * Object?replacement ^-> String
        ]

    // -----------------------------------------------------------------
    // CommandManager
    // -----------------------------------------------------------------

    let CommandManagerClass =
        CommandManager
        |+> Instance (withEvents [
            "byName" =? Object
            "commands" =? Object
            "platform" =@ String
            "exec" => String?command * Editor?editor * !?Object?args ^-> Boolean
            "exec" => Command?command * Editor?editor * !?Object?args ^-> Boolean
            "canExecute" => String?command * Editor?editor ^-> Boolean
            "canExecute" => Command?command * Editor?editor ^-> Boolean
            "toggleRecording" => Editor?editor ^-> Boolean
            "replay" => Editor?editor ^-> Boolean
            "addCommand" => Command?command ^-> Unit
            "removeCommand" => Command?command * !?Boolean?keepCommand ^-> Unit
            "removeCommand" => String?command * !?Boolean?keepCommand ^-> Unit
            "bindKey" => String?key * String?command * !?Integer?position ^-> Unit
            "bindKey" => String?key * Command?command * !?Integer?position ^-> Unit
            "addCommands" => (!|Object)?commands ^-> Unit
            "addCommands" => Object?commands ^-> Unit
            "removeCommands" => Object?commands ^-> Unit
            "bindKeys" => Object?keyList ^-> Unit
            "findKeyCommand" => Integer?hashId * String?keyString ^-> Command
            "getStatusText" => !?Editor?editor * !?Object?data ^-> String
        ])

    // -----------------------------------------------------------------
    // The global `ace` object
    // -----------------------------------------------------------------

    let AceClass =
        Ace
        |+> Static [
            "edit" => String?el * !?EditorOptions?options ^-> Editor
            "edit" => Element?el * !?EditorOptions?options ^-> Editor
            "createEditSession" => String?text * !?String?mode ^-> EditSession
            "createEditSession" => Document?text * !?String?mode ^-> EditSession
            "require" => String?moduleName ^-> Object
            "config" =? AceConfig
            "version" =? String
        ]

    let Assembly =
        Assembly [
            Namespace "WebSharper.Ace" [
                Document
                Anchor
                Range
                EditSession
                Selection
                Editor
                VirtualRenderer
                UndoManager
                Tokenizer
                BackgroundTokenizer
                TokenIterator
                Search
                CommandManager
                Command
                Position
                Delta
                Token
                Annotation
                SearchOptions
                EditorOptions
                ConfigOptions
                AceConfig
                NewLineMode
                SelectionStyle
                FoldStyle
                WrapMethod
                Ace
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member x.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()
