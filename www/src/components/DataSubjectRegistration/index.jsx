/*
  ./components/DataSubjectRegistration/index.jsx
*/
import React, { Component } from 'react';
import PropTypes from 'prop-types';
import AlertDismissable from './alertdismissable';
import { API_ROOT} from '../../api-config';

class DataSubjectRegistration extends Component {

  thinkingCount = 0;

  constructor(props) {
    super(props);
    this.state = { 
      emailaddress: '',
      isregistered: false,
      isthinking: false,
      isemailsent: false,
      isemailerror: false,
    };
    this.thinkingCount=0;
  }

  componentDidMount() { 
    this.checkAccountRegistration(this.props.address);
    this.setState = ({ 
      emailaddress: '',
      isregistered: false,
      isthinking: false,
      isemailsent: false,
      isemailerror: false,
    });
    this.thinkingCount=0;
  }

  componentDidUpdate(prevProps, prevState) {
    console.log(`Updated from ${prevProps.address} -> ${this.props.address}`);
    if (this.props.address !== prevProps.address) {
      this.checkAccountRegistration(this.props.address);
      return true;
    }
    return false;
  }

  shouldComponentUpdate(nextprops, nextstate) {
    console.log(`Account from ${this.props.address} -> ${nextprops.address}`);
    if (this.props.address !== nextprops.address) {
      return true;
    }
    return false;
  }

  toggleThinking = (isOn) => {
    if (isOn) {
      this.thinkingCount++;
      this.setState({isthinking: true});
    } else {
      this.thinkingCount > 0 ? this.thinkingCount-- : 0;
      if (this.thinkingCount === 0) {
        this.setState({isthinking: false});
      }
    }
    console.log(`Thoughts: ${this.thinkingCount}`);
  }

  checkAccountRegistration = (event) => {
    const address = event.value;
    let apiurl = `${API_ROOT}/datasubjectregistration/subjecteligible/${address}`;

    this.toggleThinking(true);

    /*
    if (true) {
      console.log('That account is already associated to an email address in Consent.Direct');
      this.setState(
        { isregistered: true }
      );
      this.toggleThinking(false);
      return;
    }
    */

    fetch(apiurl, {
      method: 'GET'
    })
    .then(response => {
      if (response.ok) {
        return response.json();
      } else {
        throw new Error("There was some kind of problem");
      }
    })
    .then(json => {
      // if success is false, it means that the subject has already registered using this account
      if (!json.success) {
        console.log('That account is already associated to an email address in Consent.Direct');
        this.setState(
          { isregistered: true }
        );
      } else {
        console.log('This account can be used to register against an email address');
        this.setState(
          { isregistered: false }
        );
      }
      this.toggleThinking(false);
    })
    .catch(error => {
      this.toggleThinking(false);
    });
  }


  handleEmailChange = (event) => {
    this.setState({ 
        emailaddress: event.target.value,
      }
    );
  }

  handleSubmit = (event) => {
    event.preventDefault();
    if (!this.canBeSubmitted()) {
      return;
    }
    const { emailaddress } = this.state;

    // Make the API call using browser fetch
    let apiurl = `${API_ROOT}/datasubjectregistration`;
    this.toggleThinking(true);
    fetch(apiurl, {
      method: 'POST',
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        EmailAddress: this.state.emailaddress,
        Account: this.state.address,
      }),
    })
    .then(response => {
      if (response.ok) {
        let ct = response.headers.get("content-type");
        if (ct && ct.includes('application/json')) {
          return response.json();
        }
        throw new TypeError("Oops, we haven't got JSON!");
      }
      throw new Error("There was some kind of problem");
    })
    .then(json => {
      console.log('Response', json);
      this.toggleThinking(false);
      if (json.success) {
        this.state.isemailsent = true;
      }
    })
    .catch(error => {
      console.error('Error posting to api', error);
      this.toggleThinking(false);
    })
  }

  canBeSubmitted = () => {
    const errors = validate(this.state.emailaddress);
    const isDisabled = Object.keys(errors).some(x => errors[x]);
    return !isDisabled;
  }

  render() {
    const formIsDisabled = this.state.isthinking || this.state.isregistered;
    const address = this.props.address;
    const errors = validate(this.state.emailaddress);
    const isSubmitDisabled = Object.keys(errors).some(x => errors[x]);
    const isregistered = this.state.isregistered;
    const isemailsent = this.state.isemailsent;
    const isemailerror = this.state.isemailerror;

    return (
      <div>
        <h2>Registering with Consent.Direct</h2>
        <p>
          So we can register your consent declarations for participating web sites we need 
          to record an Ethereum address you own, and tie that to an email address you own.
          This is all the KYC (Know Your Customer) that Consent.Direct requires!
        </p>
        <p>
          Below, you can see the number of the account that's active and unlocked in MetaMask 
          or your web3-enabled browser. Fill your email and we'll send you a 
          link that you should click to confirm your registration.
        </p>
        <p>
          Don't worry if you have no ether in the account,
          you don't need any at this stage because Consent.Direct
          will cover the gas cost for your registration.
          (this is all on the Ropsten Testnet anyway)
        </p>

        <AlertDismissable 
          style="info"
          title="Account already registered"
          text="You've already registered with Consent.Direct using that account. We can't tell you which email address has been used as this is a secret!"
          show={isregistered}
        />
        
        <AlertDismissable
          style="success"
          title="We've sent your confirmation email"
          text="You'll be receiving an email from us at consent.direct any moment now. Keep an eye on your inbox, and follow the link to complete your registration."
          show={isemailsent}
        />

        <AlertDismissable
          style="danger"
          title="Error"
          text="There was an error!"
          show={iserror}
        />

        <div className={this.state.isThinking ? '' : 'hidden'}>Thinking...</div>

        <div className={}>
          <fieldset disabled={formIsDisabled}>
            <form onSubmit={this.handleSubmit}>
                <label>
                  Your active, unlocked ethereum account
                  <input 
                    type="text" 
                    value={address} 
                    readOnly={true} 
                  />
                </label>

                <label>
                  Your email address
                  <input 
                    type="text" 
                    value={this.state.emailaddress}
                    onChange={this.handleEmailChange}
                    className={errors.emailaddress ? 'error' : ''}
                  />
                </label>

                <button disabled={isSubmitDisabled} type="submit">Send my confirmation email</button>
            </form>
          </fieldset>
        </div>
      </div>
    );
  }
}

// This could probably be moved to a utils file?
function validate (email) {
  let emailPattern = /^([a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)$/g;
  let emailIsValid = emailPattern.test(email);
  return {
    emailaddress: !emailIsValid
  }
}

export default DataSubjectRegistration;