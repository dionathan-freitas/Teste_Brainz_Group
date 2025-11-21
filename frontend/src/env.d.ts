/// <reference types="vite/client" />

// Minimal fallback for environments where `react-router-dom` types can't be resolved by the editor/tsserver.
// This prevents "Cannot find module 'react-router-dom'" in editors while keeping proper types from node_modules
// when available. If you see duplicate-declaration warnings later, remove this file and rely on the package types.

declare module 'react-router-dom' {
  export * from 'react-router-dom/dist/index';
}
