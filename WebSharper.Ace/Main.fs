namespace WebSharper.Ace

open WebSharper
open WebSharper.InterfaceGenerator

[<AutoOpen>]
module Types =
        
    let String  = T<string>
    let Element = T<JavaScript.Dom.Element>
    let Object  = T<obj>
    let Integer = T<int>
    let Boolean = T<bool>

    let T = T<unit>

module Definition =
    
    let Doc =
        Class "Document"

    let Position =
        Class "Position"
        |+> Static [
            Constructor T
        ]
        |+> Instance [
            "row" =@ Integer
            "column" =@ Integer
        ]
    
    let Anchor =
        Class "Anchor"
        |+> Static [
            Constructor (Doc * Integer * Integer)
        ]
        |+> Instance [
            "detach" => T ^-> T
            "getDocument" => T ^-> Doc
            "getPosition" => T ^-> Position
            
            "onChange" => (T ^-> T) ^-> T
            |> WithComment "Undocumented (http://ace.c9.io/#nav=api&api=anchor)"

            "setPosition" => Integer * Integer * Boolean ^-> T
        ]

    let NewLineMode =
        Pattern.EnumStrings "NewLineMode" [
            "windows"
            "unix"
            "auto"
        ]

    let EditSession =
        Class "EditSession"

    let Range =
        Class "Range"
        |+> Static [
            Constructor (Integer * Integer * Integer * Integer)
        ]
        |+> Instance [
            "start" =@ Position
            "end" =@ Position

            "isEmpty" => T ^-> Boolean
            "clipRows" => Integer * Integer ^-> TSelf
            "clone" => T ^-> TSelf
            "collapseRows" => TSelf
            "compare" => Integer * Integer ^-> Integer
            "compareEnd" => Integer * Integer ^-> Integer
            "compareInside" => Integer * Integer ^-> Integer
            "comparePoint" => TSelf ^-> Integer
            "compareRange" => TSelf ^-> Integer
            "compareStart" => Integer * Integer ^-> Integer
            "contains" => Integer * Integer ^-> Boolean
            "containsRange" => TSelf ^-> Boolean
            "extend" => Integer * Integer ^-> TSelf
            "fromPoints" => TSelf * TSelf ^-> TSelf
            "inside" => Integer * Integer ^-> Boolean
            "insideEnd" => Integer * Integer ^-> Boolean
            "insideStart" => Integer * Integer ^-> Boolean
            "intersects" => TSelf ^-> Boolean
            "isEnd" => Integer * Integer ^-> Boolean
            "isEqual" => TSelf ^-> Boolean
            "isMultiLine" => T ^-> Boolean
            "isStart" => Integer * Integer ^-> Boolean
            "setEnd" => Integer * Integer ^-> T
            "setStart" => Integer * Integer ^-> T
            "toScreenRange" => EditSession ^-> TSelf
            "toString" => T ^-> String
        ]

    let UndoManager =
        Class "UndoManager"
        |+> Static [
            Constructor T
        ]
        |+> Instance [
            "execute" => Object ^-> T
            "hasRedo" => T ^-> Boolean
            "hasUndo" => T ^-> Boolean
            "redo" => Boolean ^-> T
            "reset" => T ^-> T
            "undo" => Boolean ^-> Range
        ]

    let Document =
        Doc
        |+> Static [
            Constructor (String + Type.ArrayOf String)
        ]
        |+> Instance [
            "applyDeltas" => Object ^-> T
            "createAnchor" => Integer * Integer ^-> Anchor
            "getAllLines" => T ^-> Type.ArrayOf String
            "getLength" => T ^-> Integer
            "getLine" => T ^-> String
            "getLinea" => Integer * Integer ^-> Type.ArrayOf String
            "getNewLineCharacter" => T ^-> String
            "getNewLineMode" => T ^-> NewLineMode
            "getTextRange" => Range ^-> String
            "getValue" => T ^-> String
            "indexToPosition" => Integer * Integer ^-> Position
            "insert" => Position * String ^-> Position
            "insertInLine" => Position * String ^-> Position
            "insertLines" => Integer * Type.ArrayOf String ^-> Position
            "insertNewLine" => Position ^-> Position
            "isNewLine" => String ^-> Boolean
            "positionToIndex" => Position * Integer ^-> Integer
            "remove" => Range ^-> Position
            "removeInLine" => Integer * Integer * Integer ^-> Position
            "removeLines" => Integer * Integer ^-> String
            "removeNewLine" => Integer ^-> T
            "replace" => Range * String ^-> Position
            "revertDeltas" => Object ^-> T
            "setNewLineMode" => NewLineMode ^-> T
            "setValue" => String ^-> T
        ]

    let Selection =
        Class "Selection"

    let ES =
        EditSession
        |+> Static [
            Constructor ((Document + String) * String)
        ]
        |+> Instance [
            "addDynamicMarker" => Object * Boolean ^-> Object
            "addGutterDecoration" => Integer * String ^-> T
            "addMarker" => Range * String * ((T ^-> T) + String) * Boolean ^-> Integer
            "clearAnnotations" => T ^-> T
            "clearBreakpoint" => Integer ^-> T
            "clearBreakpoints" => T ^-> T
            "documentToScreenColumn" => Integer * Integer ^-> Integer
            "documentToScreenPosition" => Integer * Integer ^-> Position
            "documentToScreenRow" => Integer * Integer ^-> Integer
            "duplicateLines" => Integer * Integer ^-> Integer
            "getAnnotations" => T ^-> Object
            "getAWordRange" => Integer * Integer ^-> Range
            "getBreakpoints" => T ^-> Integer
            "getDocument" => T ^-> Document
            "getDocumentLastRowColumn" => Integer * Integer ^-> Integer
            "getDocumentLastRowColumnPosition" => Integer * Integer ^-> Position
            "getLength" => T ^-> Integer
            "getLine" => Integer ^-> String
            "getLines" => Integer * Integer ^-> String
            "getMarkers" => Boolean ^-> Type.ArrayOf String
            "getMode" => T ^-> String
            "getNewLineMode" => T ^-> String
            "getOverwrite" => T ^-> Boolean
            "getRowLength" => Integer ^-> Integer
            "getRowSplitData" => Object ^-> String
            "getScreenLastRowColumn" => Integer ^-> Integer
            "getScreenLength" => T ^-> Integer
            "getScreenTabSize" => Integer ^-> Integer
            "getScreenWidth" => T ^-> Integer
            "getScrollLeft" => T ^-> Integer
            "getScrollTop" => T ^-> Integer
            "getSelection" => T ^-> Selection
            "getState" => Integer ^-> String
            "getTabSize" => T ^-> Integer
            "getTabString" => T ^-> String
            "getTextRange" => Range ^-> String
            "getTokenAt" => Integer * Integer ^-> Object
            "getTokens" => Integer ^-> T
            "getUndoManager" => T ^-> UndoManager
            "getUseSoftTabs" => T ^-> Boolean
            "getUseWorker" => T ^-> Boolean
            "getUseWrapMode" => T ^-> Boolean
            "getValue" => T ^-> String
            "getWordRange" => Integer * Integer ^-> Range
            "getWrapLimit" => T ^-> Integer
            "getWrapLimitRange" => Object
            "highlight" => T ^-> T
            "highlightLines" => T ^-> T
            "indentRows" => Integer * Integer * String ^-> T
            "insert" => Position * String ^-> Position
            "isTabStop" => Position ^-> Boolean
            "moveLinesDown" => Integer * Integer ^-> Integer
            "moveLinesUp" => Integer * Integer ^-> Integer
            "moveText" => Range * Position ^-> Range
            "onChange" => (T ^-> T) ^-> T
            "onChangeFold" => (T ^-> T) ^-> T
            "onReloadTokenizer" => Object ^-> T
            "outdentRows" => Range ^-> T
            "redo" => T ^-> T
            "redoChanges" => Type.ArrayOf Object * Boolean ^-> Range
            "remove" => Range ^-> Object
            "removeGutterDecoration" => Integer * String ^-> T
            "removeMarker" => Integer ^-> T
            "replace" => Range * String ^-> Object
            "reset" => T ^-> T
            "resetCaches" => T ^-> T
            "screenToDocumentColumn" => T ^-> T
            "screenToDocumentPosition" => Integer * Integer ^-> Position
            "screenToDocumentRow" => T ^-> T
            "setAnnotations" => Type.ArrayOf Object ^-> T
            "setBreakpoint" => Integer * String ^-> T
            "setBreakpoints" => Type.ArrayOf Integer ^-> T
            "setDocument" => Document ^-> T
            "setMode" => String ^-> T
            "setNewLineMode" => String ^-> T
            "setOverwrite" => Boolean ^-> T
            "setScrollLeft" => Object ^-> T
            "setScrollTop" => Integer ^-> T
            "setTabSize" => Integer ^-> T
            "setUndoManager" => UndoManager ^-> T
            "setUndoSelect" => Boolean ^-> T
            "setUseSoftTabs" => Boolean ^-> T
            "setUseWorker" => Boolean ^-> T
            "setUseWrapMode" => Boolean ^-> T
            "setValue" => String ^-> T
            "setWrapLimitRange" => Integer * Integer ^-> T
            "toggleOverwrite" => T ^-> T
            "toString" => T ^-> String
            "undo" => T ^-> T
            "undoChanges" => Type.ArrayOf Object * Boolean ^-> Range
        ]

    let Editor =
        Class "Editor"

    let Ace =
        Class "ace"
        |> WithSourceName "Ace"
        |+> Static [
            "createEditSession" => (Document + String) * String ^-> EditSession
            "edit" => (String + Element) ^-> Editor
            "require" => String ^-> Object
        ]

    let Tokenizer =
        Class "Tokenizer"
        |+> Static [
            Constructor (Object * String)
        ]
        |+> Instance [
            "getLineTokens" => Object * Object ^-> Object
        ]

    let BackgroundTokenizer =
        Class "BackgroundTokenizer"
        |+> Static [
            Constructor (Tokenizer * Editor)
        ]
        |+> Instance [
            "fireUpdateEvents" => Integer * Integer ^-> T
            "getState" => Integer ^-> Object
            "getTokens" => Integer ^-> Object
            "setDocument" => Document ^-> T
            "setTokenizer" => Tokenizer ^-> T
            "start" => Integer ^-> T
            "stop" => T ^-> T
        ]

    let Scrollbar =
        Class "Scrollbar"
        |+> Static [
            Constructor Element
        ]
        |+> Instance [
            "getWidth" => Integer
            "onScroll" => T ^-> T
            "setHeight" => Integer ^-> T
            "setInnerHeight" => Integer ^-> T
            "setScrollTop" => Integer ^-> T
        ]

    let Search =
        Class "Search"
        |+> Static [
            Constructor T
        ]
        |+> Instance [
            "find" => EditSession ^-> Range
            "findAll" => EditSession ^-> Range
            "getOptions" => T ^-> Object
            "replace" => String * String ^-> String
            "set" => Object ^-> TSelf
            "setOptions" => Object ^-> T
        ]

    let SelectionClass =
        Selection
        |+> Static [
            Constructor EditSession
        ]
        |+> Instance [
            /// Undocumented members
            "detach" => T ^-> T
            "fromOrientedRange" => Object ^-> Object
            "getLineRange" => T ^-> Object
            "moveCursorShortWordLeft" => T ^-> T
            "moveCursorShortWordRight" => T ^-> T
            "moveCursorWordLeft" => T ^-> T
            "moveCursorWordRight" => T ^-> T
            "toggleBlockSelection" => T ^-> T
            "toOrientedRange" => T ^-> Object
            "toSingleRange" => T ^-> Object

            "addRange" => Range * Boolean ^-> T
            "clearSelection" => T ^-> T
            "getAllRanges" => T ^-> Type.ArrayOf Range
            "getCursor" => Integer
            "getRange" => T ^-> Range
            "getSelectionAnchor" => T ^-> Anchor
            "getSelectionLead" => T ^-> Position
            "getWordRange" => Object * Object ^-> Object
            "isBackwards" => T ^-> Boolean
            "isEmpty" => T ^-> Boolean
            "isMultiLine" => T ^-> Boolean
            "mergeOverlappingRanges" => T ^-> T
            "moveCursorBy" => Integer * Integer ^-> T
            "moveCursorDown" => T ^-> T
            "moveCursorFileEnd" => T ^-> T
            "moveCursorFileStart" => T ^-> T
            "moveCursorLeft" => T ^-> T
            "moveCursorLineEnd" => T ^-> T
            "moveCursorLineStart" => T ^-> T
            "moveCursorLongWordLeft" => T ^-> T
            "moveCursorLongWordRight" => T ^-> T
            "moveCursorRight" => T ^-> T
            "moveCursorTo" => Integer * Integer * Boolean ^-> T
            "moveCursorToPositon" => Position ^-> T
            "moveCursorToScreen" => T ^-> T
            "moveCursorUp" => T ^-> T
            "rectangularRangeBlock" => Object * Anchor * Boolean ^-> Range
            "selectAll" => T ^-> T
            "selectAWord" => T ^-> T
            "selectDown" => T ^-> T
            "selectFileEnd" => T ^-> T
            "selectFileStart" => T ^-> T
            "selectLeft" => T ^-> T
            "selectLine" => T ^-> T
            "selectLineEnd" => T ^-> T
            "selectLineStart" => T ^-> T
            "selectRight" => T ^-> T
            "selectTo" => Integer * Integer ^-> T
            "selectToPosition" => Position ^-> T
            "selectUp" => T ^-> T
            "selectWord" => T ^-> T
            "selectWordLeft" => T ^-> T
            "selectWordRight" => T ^-> T
            "setSelectionAnchor" => Integer * Integer ^-> T
            "setSelectionRange" => Range * Boolean ^-> T
            "shiftSelection" => Integer ^-> T
            "splitIntoLines" => T ^-> T
            "substractPoint" => Range ^-> T
        ]

    let TokenIterator =
        Class "TokenIterator"
        |+> Static [
            Constructor (EditSession * Integer * Integer)
        ]
        |+> Instance [
            "getCurrentToken" => T ^-> String
            "getCurrentTokenColumn" => T ^-> Integer
            "getCurrentTokenRow" => T ^-> Integer
            "stepBackward" => T ^-> String
            "stepForward" => T ^-> String
        ]

    let VirtualRenderer =
        Class "VirtualRenderer"
        |+> Static [
            Constructor (Element * String)
        ]
        |+> Instance [
            /// Undocumented members
            "_loadTheme" => Object ^-> T
            "alignCursor" => T ^-> T
            "animateScrolling" => T ^-> T
            "getDisplayIndentGuides" => T ^-> Object
            "getFadeFoldWidgets" => T ^-> Object
            "getHighlightGutterLine" => T ^-> Object
            "onChangeTabSize" => Object ^-> T
            "onGutterResize" => Object ^-> T
            "pixelToScreenCoordinates" => Object ^-> Object
            "screenToTextCoordinates" => Object ^-> Object
            "scrollSelectionIntoView" => T ^-> T
            "setCompositionText" => String ^-> T
            "setDisplayIndentGuides" => T ^-> T
            "setFadeFoldWidgets" => Object ^-> T
            "setHighlightGutterLine" => Object ^-> T
            "setStyle" => Object ^-> T
            "updateCharacterSize" => Object ^-> T

            "adjustWrapLimit" => T ^-> T
            "destroy" => T ^-> T
            "getAnimatedScroll" => T ^-> Boolean
            "getContainerElement" => T ^-> Element
            "getFirstFullyVisibleRow" => T ^-> Integer
            "getFirstVisibleRow" => T ^-> Integer
            "getHScrollBarAlwaysVisible" => T ^-> Boolean
            "getLastFullyVisibleRow" => Integer
            "getLastVisibleRow" => Integer
            "getMouseEventTarget" => T ^-> Element
            "getPrintMarginColumn" => T ^-> Boolean
            "getScrollBottomRow" => T ^-> Integer
            "getScrollLeft" => T ^-> Integer
            "getScrollTop" => T ^-> Integer
            "getScrollTopRow" => T ^-> Integer
            "getShowGutter" => T ^-> Boolean
            "getShowInvisibles" => T ^-> Boolean
            "getShowPrintMargin" => T ^-> Boolean
            "getTextAreaContainer" => T ^-> Element
            "getTheme" => T ^-> String
            "hideComposition" => T ^-> T
            "hideCursor" => T ^-> T
            "isScrollableBy" => Integer * Integer ^-> Boolean
            "onResize" => Boolean * Integer * Integer * Integer ^-> T
            "scrollBy" => Integer ^-> Integer
            "scrollCursorIntoView" => Object * Object ^-> T
            "scrollToLine" => Integer * Boolean * Boolean * (T ^-> T) ^-> T
            "scrollToRow" => Integer ^-> T
            "scrollToX" => Integer ^-> Integer
            "scrollToY" => Integer ^-> Integer
            "setAnimatedScroll" => Boolean ^-> T
            "setAnnotations" => Type.ArrayOf Object ^-> T
            "setScrollBarAlwaysVisible" => Boolean ^-> T
            "setPadding" => Integer ^-> T
            "setPrintMarginColumn" => Boolean ^-> T
            "setSession" => Object ^-> T
            "setShowGutter" => Boolean ^-> T
            "setShowInvisibles" => Boolean ^-> T
            "setShowPrintMargin" => Boolean ^-> T
            "setTheme" => String ^-> T
            "showCursor" => T ^-> T
            "textToScreenCoordinates" => Integer * Integer ^-> Object
            "unsetStyle" => String ^-> T
            "updateBackMarkers" => T ^-> T
            "updateBreakpoints" => Object ^-> T
            "updateCursor" => T ^-> T
            "updateFontSize" => Integer ^-> T
            "updateFontMarkers" => T ^-> T
            "updateFull" => Boolean ^-> T
            "updateLines" => Integer * Integer ^-> T
            "updateText" => T ^-> T
            "visualizeBlur" => T ^-> T
            "visualizeFocus" => T ^-> T
        ]

    let Keybinding =
        Pattern.Config "Keybinding" {
            Required = []
            Optional =
                [
                    "win", String
                    "mac", String
                ]
        }

    let CommandConfig =
        Pattern.Config "CommandConfig" {
            Required = []
            Optional =
                [
                    "name", String
                    "bindKey", Keybinding.Type
                    "exec", (Editor ^-> T)
                ]
        }

    let CommandCollection =
        Class "CommandCollection"
        |+> Instance [
            "addCommand" => CommandConfig ^-> T
        ]

    let EditorClass =
        Editor
        |+> Static [
            Constructor (VirtualRenderer * ES)
        ]
        |+> Instance [
            "commands" =? CommandCollection

            "addSelectionMarker" => Range ^-> Range
            "alignCursors" => T ^-> T
            "blockOutdent" => T ^-> T
            "blur" => T ^-> T
            "centerSelection" => T ^-> T
            "clearSelection" => T ^-> T
            "copyLinesDown" => T ^-> Integer
            "copyLinesUp" => T ^-> Integer
            "setValue" => String * !? Integer ^-> T
            "getValue" => T ^-> String
            "getSession" => T ^-> EditSession
            "focus" => T ^-> T
            "gotoLine" => Integer * Integer * !? Boolean ^-> T
            "on" => String * (Object ^-> T) ^-> T
            "resize" => !? Boolean ^-> T
            "getCursorPosition" => T ^-> Position
        ]

    let cdn resource =
        "http://cdn.jsdelivr.net/ace/1.2.2/min/" + resource + ".js"

    let Assembly =
        Assembly [
            Namespace "WebSharper.Ace" [
                 Document
                 Ace
                 Editor
                 VirtualRenderer
                 EditSession
                 Range
                 UndoManager
                 Position
                 NewLineMode
                 Anchor
                 Selection
                 CommandCollection
                 CommandConfig
                 Keybinding
            ]
            Namespace "WebSharper.Ace.Resources" [
                Resource "Ace" (cdn "ace")
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member x.Assembly =
            Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()
