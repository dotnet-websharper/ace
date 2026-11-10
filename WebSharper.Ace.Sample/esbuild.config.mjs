import { build } from 'esbuild'
import { readFileSync, writeFileSync, mkdirSync, copyFileSync } from 'fs'

const name = 'WebSharper.Ace.Sample'

await build({
  entryPoints: [`./build/${name}.js`],
  bundle: true,
  minify: true,
  format: 'iife',
  outfile: `../dist/Scripts/${name}.min.js`,
  globalName: 'wsbundle'
})

mkdirSync('../dist/css', { recursive: true })
copyFileSync('./css/index.css', '../dist/css/index.css')

// Inject the WebSharper head into the page. This is where the binding's
// AssemblyWide Ace resource is emitted as a <script> tag, so the sample loads
// the very Ace build the binding is written against.
const head = readFileSync(`./build/${name}.head.html`, 'utf8')
const html = readFileSync('./index.html', 'utf8').replace('<!-- ws-head -->', head)
writeFileSync('../dist/index.html', html)

console.log('Bundled:', name)
