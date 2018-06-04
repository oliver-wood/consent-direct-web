/*
  ./components/App.jsx
*/
import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { Link, Route } from 'react-router-dom';
import About from './About';
import DataSubjectRegistration from './DataSubjectRegistration';


class App extends Component {
  
  constructor(props, context) {
    super(props, context);
    this.state = {
      web3: context.web3,
    }
  }

  static getDerivedStateFromProps(props, state) {
    console.log('Deriving state from props in App ' + props.newaccount);
    if (props.newaccount && props.newaccount !== '') {
      return { 
        web3: {
          ...state.web3, selectedAccount: props.newaccount
        } 
      }
    }
    return {};
  }

  render() {
    const address = this.state.web3.selectedAccount;
    return (
      <div>
        <nav>
          <h1>Consent.Direct</h1>
          <ul>
            <li><Link to="/">For Data Subjects</Link></li>
            <li><Link to="/data-processors">For Data Processors</Link></li>
            <li><Link to="/about">About Consent.Direct</Link></li>
          </ul>
        </nav>
        <div>
          <p>Some standard text here</p>
          <Route path="/" render={(props) => 
            <DataSubjectRegistration
              address={address}
            />
          } />
          <Route path="/about" component={About} />
        </div>
      </div>
    );
  }
}

const contextTypes = {
  web3: PropTypes.object.isRequired,
};
App.contextTypes = contextTypes;

export default App;