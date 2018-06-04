let apiHost;
// const apiVersion = 'v1';

const hostname = window && window.location && window.location.hostname;

if (hostname === 'consent.direct') {
  apiHost = 'https://consent.direct';
} else {
  apiHost = process.env.REACT_APP_API_HOST || 'http://localhost:5000';
}

export const API_ROOT = `${apiHost}/api`;