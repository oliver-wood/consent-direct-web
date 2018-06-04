/*
  ./components/Web3Wrapper.jsx
*/
import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { Web3Provider } from 'react-web3';
import App from './App.jsx';
import { Web3Unavailable, Web3AccountUnavailable } from './Web3Fallback';


class Web3Wrapper extends Component {
  constructor(props) {
    super(props);
    this.state = {
      newaccount: ''
    }
  }

  onChangeAccount = (newaddress) => {
    this.setState({newaccount: newaddress});
  }

  render() {
    return (
      <Web3Provider
          web3UnavailableScreen={() => <Web3Unavailable /> }
          accountUnavailableScreen={() => <Web3AccountUnavailable />}
          onChangeAccount={ newaddress => this.onChangeAccount(newaddress)}
        >
        <App newaccount={this.state.newaccount}/>
      </Web3Provider>
    );
  }
}

export default Web3Wrapper;