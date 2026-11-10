# WebSharper.Ace — API discrepancies and modernization notes

This document records the differences between the original (2018-era) binding
and the public Ace API, as published with `ace-builds` 1.45.x in
<https://github.com/ajaxorg/ace-builds/tree/master/types> (in particular the
`Ace` namespace in `ace.d.ts`, and the classes in `types/ace-modules.d.ts`).

The binding was updated **incrementally**: the existing surface is kept where it
was correct, wrong members are fixed, and missing members are added. Where a
published signature could not be modelled faithfully, the deviation is listed at
the end.

Legend: **fixed** = wrong in the old binding, corrected; **added** = present in
Ace but missing before; **kept** = already matched Ace.

---

## The `ace` object

| Member | Old binding | Ace 1.45 | Now |
| --- | --- | --- | --- |
| source name | `Class "ace"` with `WithSourceName "Ace"` | the `ace` export | kept (JS name `ace`, imported) |
| `edit` | `(string + Element) -> Editor` only | `edit(el?: string \| HTMLElement \| null, options?) -> Editor` | **fixed**: `Edit(string, options?)` and `Edit(Element, options?)`; the string is an element **id** (`getElementById`) |
| `createEditSession` | `(Document + string) * string -> EditSession`, mode required | `createEditSession(text: Document \| string, mode?) -> EditSession` | **fixed**: two overloads, optional mode |
| `require` | `string -> obj` | `require(name: string): any` | kept |
| `config` | missing | `ace.config` (`get`, `set`, `all`, `moduleUrl`, `setModuleUrl`, `setLoader`, `loadModule`, `version`, …) | **added** (`Config` / `ConfigOptions`) |
| `version` | missing | `"1.45.2"` | **added** |

## `Point` / `Position`

| | Old binding | Ace 1.45 | Now |
| --- | --- | --- | --- |
| shape | `Class "Position"` with a bogus `new Position()` constructor | plain `{ row; column }` (`Point`) | **fixed**: `Pattern.Config`, so `Position(row, column)` emits an object literal |

## `Range`

| Member | Old binding | Ace 1.45 | Now |
| --- | --- | --- | --- |
| constructor | `(int * int * int * int)`, all required | all four optional | **fixed**: `Range()` and `Range(r1,c1,r2,c2)` |
| `fromPoints` | instance method `(Range * Range) -> Range` | **static** `fromPoints(start, end): Range` | **fixed** (static, takes two points) |
| `comparePoints` | missing | static `comparePoints(p1, p2): number` | **added** |
| `comparePoint` | `(Range) -> int` | `(Point) -> number` | **fixed** |
| `compareRange`, `isEqual`, `containsRange`, `intersects` | `(Range)` | `(IRange)` | kept (takes `Range`; any `{start,end}` is accepted at runtime) |
| `collapseRows` | declared as a value, not a method | `collapseRows(): Range` | **fixed** |
| `setStart` / `setEnd` | `(int * int)` | `(row: number \| Point, column?, noClip?)` | **fixed**: `(int, int, noClip?)` |
| `toScreenRange` | `(EditSession) -> Range` | kept | kept |
| `moveBy` | missing | `moveBy(row, column)` | **added** |
| `start`, `end`, `id`, `cursor`, `isBackwards` | partial | present | **added** |

## `Document`

| Member | Old binding | Ace 1.45 | Now |
| --- | --- | --- | --- |
| constructor | `(string + string[])` | `new Document(string \| string[])` | binding exposes no constructor (not exported by `ace-builds`) |
| `getLinea` | **typo**, `(int * int) -> string[]` | `getLines(firstRow, lastRow): string[]` | **fixed** (renamed `GetLines`) |
| `getTextRange` | `(Range) -> string` | `(IRange) -> string` | kept |
| `removeLines` | `-> string` | `-> string[]` | **fixed** |
| `insertNewLine`, `insertLines` | present | **deprecated** by Ace | **kept + `Obsolete`** |
| `getLinesForRange`, `insertFullLines`, `insertMergedLines`, `removeFullLines`, `clippedPos`, `clonePos`, `pos`, `applyDelta`, `revertDelta` | missing | present | **added** |
| `applyDeltas` / `revertDeltas` | `obj` | `Delta[]` | **fixed** |
| `remove` / `replace` | `-> obj` | `-> Point` | **fixed** |
| `on`/`off`/… | `onChange` | `EventEmitter` | **fixed**: full emitter surface |

## `Anchor`

| Member | Old binding | Ace 1.45 | Now |
| --- | --- | --- | --- |
| constructor | `(Document, int, int)` required | `(doc, row: number \| Point, column?)` | binding exposes no constructor (use `doc.CreateAnchor`) |
| `onChange` | `(unit -> unit) -> unit` | `EventEmitter` `"change"` | **fixed** |
| `attach`, `document`, `markerId` | missing | present | **added** |
| `setPosition` | `(int, int, bool)` | `(row, column, noClip?)` | **fixed** (third arg optional) |

## `EditSession`

| Member | Old binding | Ace 1.45 | Now |
| --- | --- | --- | --- |
| constructor | `(Document + string) * string`, mode required | `(Document \| string, mode?)` | **fixed** (two overloads, optional mode) |
| `fromJSON` | missing | `static fromJSON(session): EditSession` | **added** |
| `getMode` | `-> string` | `-> SyntaxMode` | **fixed** (`-> obj`; mode objects are not modelled) |
| `getNewLineMode` | `-> string` | `-> NewLineMode` | **fixed** (`NewLineMode`) |
| `setNewLineMode` | `string` | `NewLineMode` | **fixed** |
| `getMarkers` | `-> string[]` | `-> { [id]: MarkerLike }` | **fixed** (`-> obj`) |
| `getAnnotations` | `-> obj` | `-> Annotation[]` | **fixed** |
| `getBreakpoints` | `-> int` | `-> string[]` | **fixed** |
| `remove` / `replace` | `-> obj` | `-> Point` | **fixed** |
| `undoChanges` / `redoChanges` | `-> Range` | `-> void` | **fixed** |
| `screenToDocumentColumn` / `Row` | `unit -> unit` | `(row, column) -> number` | **fixed** |
| `documentToScreenPosition` etc. | partial | `(row \| Point, column?)` | **fixed** / **added** |
| `addMarker` | `(Range, string, (unit -> unit) + string, bool)` | `(range, class, type?, inFront?)` | kept as two overloads (string / renderer function) |
| `moveText` | no `copy` argument | `(range, pos, copy?)` | **fixed** |
| `getWordRange`, `getAWordRange`, `getLine`, `getLines`, `getLength`, `getTextRange`, `setMode`, `setTabSize`, `getTabSize`, `setUseSoftTabs`, `getUseSoftTabs`, `setOverwrite`, `getOverwrite`, `toggleOverwrite`, `setUndoManager`, `getUndoManager`, `markUndoGroup`, `setScrollTop`/`getScrollTop`, `setScrollLeft`/`getScrollLeft`, `getScreenWidth`, `getScreenLength`, `getRowLength`, `adjustWrapLimit`, `duplicateLines`, `indentRows`, `outdentRows`, `getPrecedingCharacter`, `destroy`, `setDocument`, `getDocument`, `resetCaches`, `setValue`, `getValue`, `highlightLines`, `addGutterCustomWidget`, `getSelectionMarkers`, `toJSON` | missing | present | **added** |

## `Selection`

The old binding left `Selection` empty and put the members on a separate
`SelectionClass` type that was **not in the assembly**, so they were unreachable.
The members now live on `Selection`.

| Member | Old binding | Ace 1.45 | Now |
| --- | --- | --- | --- |
| `getCursor` | `-> int` | `-> Point` | **fixed** |
| `getSelectionLead`, `getSelectionAnchor` | `-> Position` / `-> Anchor` | `-> Point` | **fixed** |
| `moveCursorToPositon` | **typo** | `moveCursorToPosition` | **fixed** |
| `toOrientedRange` / `fromOrientedRange` | `obj` | `Range` | **fixed** |
| `rectangularRangeBlock` | `(obj, Anchor, bool) -> Range` | `(screenCursor, screenAnchor, includeEmptyLines?) -> Range[]` | **fixed** |
| `setAnchor`, `getAnchor`, `setRange`, `moveTo`, `moveToPosition`, `getAllRanges`, `mergeOverlappingRanges`, `splitIntoLines`, `joinSelections`, `toggleBlockSelection`, `toSingleRange`, `wouldMoveIntoSoftTab`, `getRangeOfMovements`, `toJSON`, `fromJSON`, `isEqual`, multi-select properties | missing | present | **added** |
| events | missing | `changeCursor`, `changeSelection`, `multiSelect`, `addRange`, `removeRange`, `singleSelect` | **added** (via `On`) |

## `Editor`

| Member | Old binding | Ace 1.45 | Now |
| --- | --- | --- | --- |
| `commands` | `CommandCollection` | `CommandManager` | **fixed** |
| `copyLinesDown` / `copyLinesUp` | `-> int` | `-> void` | **fixed** |
| `setValue` | `-> unit` | `(val, cursorPos?) -> string` | **fixed** |
| `on` | string callback | `EventEmitter` | **fixed** |
| `session`, `selection`, `renderer`, `container`, `id` | missing / partial | present | **added** |
| `setTheme`, `getTheme`, `setFontSize`, `getFontSize`, `setReadOnly`, `getReadOnly`, `isFocused`, `destroy`, `getCopyText`, `execCommand`, `insert`, `undo`, `redo`, `find`, `findNext`, `findPrevious`, `replace`, `replaceAll`, `getSelection`, `getSelectionRange`, `selectAll`, `clearSelection`, `moveCursorTo`, `gotoLine`, `navigateTo`, navigation, options (`setOptions`/`getOptions`/`setOption`/`getOption`), multi-select helpers | missing | present | **added** |

## `VirtualRenderer`

| Member | Old binding | Ace 1.45 | Now |
| --- | --- | --- | --- |
| `getPrintMarginColumn` | `-> bool` | `-> number` | **fixed** |
| `getLastFullyVisibleRow` / `getLastVisibleRow` | declared as values | methods | **fixed** |
| `setScrollBarAlwaysVisible` | old name | `setHScrollBarAlwaysVisible` / `setVScrollBarAlwaysVisible` | **fixed** |
| `scrollBy` / `scrollToX` / `scrollToY` | `-> int` | `-> void` | **fixed** |
| `_loadTheme`, `onResize`, `onChangeTabSize`, `onGutterResize` | private/internal | not public | **removed** |
| `updateFontSize` | `(int)` | `()` | **fixed** |
| `setSession`, `setAnnotations`, `setPadding`, `textToScreenCoordinates`, `pixelToScreenCoordinates`, `screenToTextCoordinates`, `freeze`/`unfreeze`, `setScrollMargin`, `setMargin`, `updateFrontMarkers`/`updateBackMarkers`, `setCursorStyle`, `attachToShadowRoot`, composition and ghost-text helpers | partial / missing | present | **added** |

## `UndoManager`

| Member | Old binding | Ace 1.45 | Now |
| --- | --- | --- | --- |
| `execute` | `obj -> unit` | not part of the public API | **removed** (replaced by `add`) |
| `undo` / `redo` | `bool -> Range` / `bool -> unit` | `(session, dontSelect?) -> void` | **fixed** |
| `hasUndo` / `hasRedo` | methods | methods (`() => boolean`) | kept |
| `canUndo`, `canRedo`, `reset`, `getRevision`, `getDeltas`, `addSession`, `bookmark`, `isClean`, `markClean`, `markIgnored`, `startNewGroup`, `getSelection`, `toJSON`, `fromJSON` | missing | present | **added** |

## `Tokenizer` / `BackgroundTokenizer` / `TokenIterator`

| Member | Old binding | Ace 1.45 | Now |
| --- | --- | --- | --- |
| `Tokenizer` constructor | `(obj, string)` | `(rules)` | **fixed** (ctor not exposed: not exported by `ace-builds`) |
| `Tokenizer.getLineTokens` | `(obj, obj) -> obj` | `(line, state) -> { tokens, state }` | **fixed** |
| `BackgroundTokenizer` constructor | `(Tokenizer, Editor)` | `(tokenizer, session?)` | fixed (ctor not exposed) |
| `fireUpdateEvents` | **typo + plural** | `fireUpdateEvent(firstRow, lastRow)` | **fixed** |
| `start`, `getState`, `getTokens`, `scheduleStart`, `cleanup` | partial/missing | present | **fixed/added** |
| `TokenIterator` constructor | `(EditSession, int, int)` | present, but not exported by `ace-builds` | binding exposes no constructor (use `session.GetTokenAt`) |
| `stepForward` / `stepBackward` / `getCurrentToken` | `-> string` | `-> Token \| null` | **fixed** (`Token`) |
| `getCurrentTokenPosition`, `getCurrentTokenRange` | missing | present | **added** |

## `Search`

| Member | Old binding | Ace 1.45 | Now |
| --- | --- | --- | --- |
| `find` | `(EditSession) -> Range` | `(session) -> Range \| null \| false` | **fixed** |
| `findAll` | `-> Range` | `-> Range[]` | **fixed** |
| `getOptions` / `setOptions` / `set` | `obj` | `SearchOptions` | **fixed** |
| `parseReplaceString` | missing | present | **added** |

## `Command` / `CommandManager`

| Member | Old binding | Ace 1.45 | Now |
| --- | --- | --- | --- |
| type name | `CommandConfig` | `Command` | **renamed** |
| `bindKey` | `Keybinding` object only | `string \| { mac, win }` | **fixed** (`string`; the object form is dropped) |
| `isAvailable`, `description`, `multiSelectAction`, `scrollIntoView`, `aceCommandGroup`, `passEvent`, `level`, `action`, `readOnly` | missing | present | **added** |
| `CommandCollection` | `addCommand` only | `CommandManager` (`addCommand`, `removeCommand`, `bindKey`, `exec`, `canExecute`, `byName`, `commands`, …) | **renamed + expanded** |

---

## Deliberate deviations

These are places where the binding does not (or cannot) mirror the published
types exactly:

1. **Union parameters become overloads.** WIG's `A | B` generates an explicit
   `Union<'a,'b>` (constructed with `Union1Of2`/`Union2Of2`), which is not
   ergonomic at F# call sites. Overloads are used for `edit`,
   `createEditSession`, `find`, `execCommand`, `setFontSize`, command manager
   methods, etc.
2. **`IRange` is modelled as `Range`.** Methods that accept any `{ start; end }`
   object take a `Range`; a plain object still works at runtime.
3. **`SyntaxMode` / theme objects are not modelled.** Mode and theme parameters
   are strings (`"ace/mode/fsharp"`, `"ace/theme/monokai"`); `getMode()` returns
   `obj`.
4. **Only the classes exported by `ace-builds` are constructible.** `Range`,
   `Editor`, `EditSession`, `UndoManager` and `VirtualRenderer` are exported by
   the package and have constructors. `Document`, `Anchor`, `Selection`,
   `Tokenizer`, `BackgroundTokenizer`, `TokenIterator`, `Search` and
   `CommandManager` are obtained from the editor/session/document.
5. **Event payloads are `obj`.** Ace's typed event maps are documented in
   `ace.d.ts`; the binding exposes `On(name, obj -> unit)`.
6. **`Document.getRowSplitData`** is typed `string`, though Ace can return
   `undefined`.
7. **`Selection.toJSON`** is typed `Range`, though Ace can return `Range[]`.
8. **`Editor.find`** is typed `Range`, though Ace can return `false`/`null`.

## Packaging: npm import instead of a CDN `<script>`

The old binding linked a single `ace.js` from a CDN as an `AssemblyWide`
resource (`http://cdn.jsdelivr.net/ace/1.2.2/min/ace.js`). The binding now
imports the npm package [`ace-builds`](https://www.npmjs.com/package/ace-builds):

- `ace` is `import * as ace from "ace-builds/src-noconflict/ace"`;
- `Range`, `Editor`, `EditSession`, `UndoManager` and `VirtualRenderer` are
  named imports from the same module;
- the binding declares `ace-builds` in `NpmDependencies`, so consumers install
  it from npm and bundle it (esbuild in the sample).

No `<script>` tag is emitted. Modes, themes and workers are the consumer's
responsibility: import the `mode-*`/`theme-*` modules (see `entry.mjs` in the
sample) and either import or disable the background workers
(`EditorOptions.UseWorker <- false`).
