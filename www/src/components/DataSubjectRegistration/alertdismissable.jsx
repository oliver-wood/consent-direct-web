import React, {Component } from 'react';
import Alert from 'react-bootstrap/lib/Alert';
import Button from 'react-bootstrap/lib/Button';


class AlertDismissable extends React.Component {
  constructor(props, context) {
    super(props, context);

    this.handleDismiss = this.handleDismiss.bind(this);
    this.handleShow = this.handleShow.bind(this);

    this.state = {
      show: props.show
    };
  }
  
  componentWillReceiveProps(newProps) {
    this.setState({show: newProps.show});
  }

  handleDismiss() {
    this.setState({ show: false });
  }

  handleShow() {
    this.setState({ show: true });
  }

  render() {
    if (this.state.show) {
      return (
        <Alert bsStyle={this.props.style} onDismiss={this.handleDismiss}>
          <h4>{this.props.title}</h4>
          <p>{this.props.text}</p>
          <p>
            <Button bsStyle={this.props.style}>Take this action</Button>
            <span> or </span>
            <Button onClick={this.handleDismiss}>Hide Alert</Button>
          </p>
        </Alert>
      );
    } else {
      return null;
    }
  }
}

export default AlertDismissable;