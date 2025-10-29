// Set HTMX configuration across application

// This will force sending form data via Query Param only for HTTPGet
// By specification form data is sent via query param for HTTPDelete.  But this causes an issue with Anti-Forgery Token
// Therefore we force all HTTPDelete with form data to go via the body
// See: https://htmx.org/migration-guide-htmx-1/
htmx.config.methodsThatUseUrlParams = ["get"];