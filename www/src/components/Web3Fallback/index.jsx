import React, { Component } from 'react';

export class Web3Unavailable extends Component {
  render() {
    return (
      <div>
        <h1>You need the MetaMask plugin!</h1>
        <p>
            Please download and install the Metamask browser plugin, 
            or load this DAPP in a web3-enabled browser.
        </p>
      </div>
    );
  }
}

export class Web3AccountUnavailable extends Component {
  render() {
    return (
      <div>
        <h1>None of your accounts are unlocked!</h1>
        <p>
            Please unlock an Ethereum-enabled account in your web3 client to get started.
        </p>
      </div>
    );
  }
}
