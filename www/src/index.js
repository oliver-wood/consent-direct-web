/*
    ./index.js
    The webpack entry file
*/
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import { AppContainer } from 'react-hot-loader';
import Web3Wrapper from './components/Web3Wrapper.jsx';

require('./stylesheets/app.sass');

const render = Component => {
  ReactDOM.render(
    <AppContainer>
      <BrowserRouter>
        <Component />
      </BrowserRouter>
    </AppContainer>,
    document.getElementById('root')
  );
}
render(Web3Wrapper);
  

if (module.hot) {
  module.hot.accept('./components/Web3Wrapper.jsx', () => { render(Web3Wrapper) });
}