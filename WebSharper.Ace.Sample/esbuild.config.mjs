import { build } from 'esbuild'
import { readFileSync, writeFileSync, mkdirSync, copyFileSync } from 'fs'

const name = 'WebSharper.Ace.Sample'

// `entry.mjs` imports the npm `ace-builds` core plus the modes/themes the
// sample uses, then the WebSharper-generated bundle.
await build({
  entryPoints: ['./entry.mjs'],
  bundle: true,
  minify: true,
  format: 'iife',
  outfile: `../dist/Scripts/${name}.min.js`,
  globalName: 'wsbundle'
})

mkdirSync('../dist/css', { recursive: true })
copyFileSync('./css/index.css', '../dist/css/index.css')

// Fold the WebSharper head into the page. Ace is bundled from npm, so the head
// is normally empty, but this keeps the mechanism for any linked resources.
const head = readFileSync(`./build/${name}.head.html`, 'utf8')
const html = readFileSync('./index.html', 'utf8').replace('<!-- ws-head -->', head)
writeFileSync('../dist/index.html', html)

console.log('Bundled:', name)
