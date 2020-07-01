export const server = 'https://localhost:5001';

export const webAPIUrl = `${server}/api`;

export const authSettings = {
  domain: 'dev-w9siw-1w.eu.auth0.com/',
  client_id: 'lzhbi6YD27pVJL4HQIeYHIU1r4AeKI6y',
  redirect_uri: window.location.origin + '/signin-callback',
  scope: 'openid profile QandAAPI email',
  audience: 'https://qanda',
};
