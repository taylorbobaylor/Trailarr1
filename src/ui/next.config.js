const nextConfig = { reactStrictMode: true, webpack: (config) => { config.resolve.alias = { ...config.resolve.alias, "@": "/src", }; return config; }, }; module.exports = nextConfig;
