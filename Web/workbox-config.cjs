module.exports = {
  globDirectory: "../Builds/WebGL/",
  globPatterns: ["**/*.{html,js,css,json,webmanifest,png,unityweb}"],
  globIgnores: ["ServiceWorker.js", "workbox-*.js"],
  swDest: "../Builds/WebGL/ServiceWorker.js",
  maximumFileSizeToCacheInBytes: 20 * 1024 * 1024,
  dontCacheBustURLsMatching: /Build\/[0-9a-f]{32}\./,
  skipWaiting: true,
  clientsClaim: true,
  cleanupOutdatedCaches: true,
  sourcemap: false
};
