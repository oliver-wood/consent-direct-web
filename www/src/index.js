/*
    ./client/index.js
    which is the webpack entry file
*/
import React from 'react';
import ReactDOM from 'react-dom';
import App from './components/App.jsx';
import { AppContainer } from 'react-hot-loader';

require('./stylesheets/app.sass');

const render = Component => {
    ReactDOM.render(
        <AppContainer>
            <Component />
        </AppContainer>,
        document.getElementById('root')
    );
}
render(App);
  

if (module.hot) {
    module.hot.accept('./components/App.jsx', () => { render(App) });
}
// ReactDOM.render(<App />, document.getElementById('root'));
