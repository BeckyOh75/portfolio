import React from "react";
import { Formik, Field } from "formik";
import LocationBasicForm from "../location/LocationBasicForm";
import * as locationService from "../../services/locationService";
import * as therapistsService from "../../services/therapistsService";
import Swal from "sweetalert2";
import therapistValidationSchema from "./therapistValidationSchema";
import { withRouter } from "react-router-dom";
import PropTypes from "prop-types";

class AddTherapist extends React.Component {
  state = {
    formData: {
      email: "",
      firstName: "",
      mi: "",
      lastName: "",
      avatarUrl: "",
      phone: "",
      location: {
        locationTypeId: 0,
        lineOne: "",
        lineTwo: "",
        city: "",
        zip: "",
        stateId: 0,
      },
      locationId: 0,
      isActive: true,
      notes: "",
    },
    hasLocation: false,
  };

  handleSubmit = (values, { resetForm }) => {
    const payload = { ...values };

    if (this.state.hasLocation) {
      locationService.getGeocode(values.location).then((response) => {
        payload.location.latitude = response.latitude;
        payload.location.longitude = response.longitude;
        payload.location.stateId = parseInt(
          values.location.stateId.split("-")[1]
        );
        payload.location.locationTypeId = parseInt(
          values.location.locationType.id
        );

        locationService
          .addLocation(payload.location)
          .then((response) => this.onAddLocationSuccess(payload, response.item))
          .catch((response) => this.onAddLocationFailure(response));
      });
    } else {
      therapistsService
        .addTherapist(payload)
        .then(this.onAddTherapistSuccess)
        .catch(this.onAddTherapistFailure);
    }

    resetForm(this.state.formData);
  };

  onAddLocationSuccess = (data, locationId) => {
    data.locationId = locationId;
    therapistsService
      .addTherapist(data)
      .then(this.onAddTherapistSuccess)
      .catch(this.onAddTherapistFailure);
  };

  onAddLocationFailure = () => {
    Swal.fire("Location is invalid.", "Please try again.", "error");
  };

  onAddTherapistSuccess = () => {
    Swal.fire("Success", "Your record has been successfully added!", "success");
    this.props.history.push("/therapists");
  };

  onAddTherapistFailure = () => {
    Swal.fire(
      "There was an unexpected error.",
      "Please try again later.",
      "error"
    );
  };

  toggleLocation = () =>
    this.setState((prevState) => ({
      ...prevState,
      hasLocation: !prevState.hasLocation,
    }));

  collapseLocation = () =>
    this.setState((prevState) => ({
      ...prevState,
      hasLocation: !prevState.hasLocation,
    }));

  render() {
    return (
      <React.Fragment>
        <Formik
          enableReinitialize={true}
          validationSchema={therapistValidationSchema}
          initialValues={this.state.formData}
          onSubmit={this.handleSubmit}
        >
          {(props) => {
            const {
              values,
              touched,
              errors,
              handleSubmit,
              isValid,
              isSubmitting,
            } = props;
            return (
              <div className="container pt-5 pb-1">
                <div className="card">
                  <div className="card-header">
                    <h3>Add Therapist</h3>
                  </div>
                  <div className="card-body">
                    <form onSubmit={handleSubmit}>
                      <div className="form-row">
                        <div className="form-group col-md-5">
                          <label htmlFor="LastName">Last Name</label>
                          <Field
                            name="lastName"
                            type="text"
                            values={values.lastName}
                            placeholder="Last Name"
                            autoComplete="off"
                            className="form-control"
                          />
                          {errors.lastName && touched.lastName && (
                            <span className="input-feedback text-danger">
                              {errors.lastName}
                            </span>
                          )}
                        </div>
                        <div className="form-group col-md-4">
                          <label htmlFor="FirstName">First Name</label>
                          <Field
                            name="firstName"
                            type="text"
                            values={values.firstName}
                            placeholder="First Name"
                            autoComplete="off"
                            className="form-control"
                          />
                          {errors.firstName && touched.firstName && (
                            <span className="input-feedback text-danger">
                              {errors.firstName}
                            </span>
                          )}
                        </div>
                        <div className="form-group col-md-3">
                          <label htmlFor="mi">Middle Initial</label>
                          <Field
                            name="mi"
                            type="text"
                            values={values.mi}
                            placeholder="Middle Initial"
                            autoComplete="off"
                            className="form-control"
                          />
                          {errors.mi && touched.mi && (
                            <span className="input-feedback text-danger">
                              {errors.mi}
                            </span>
                          )}
                        </div>
                      </div>
                      <div className="form-row">
                        <div className="form-group col-md-6">
                          <label htmlFor="email">Email</label>
                          <Field
                            name="email"
                            type="text"
                            values={values.email}
                            placeholder="Email"
                            autoComplete="off"
                            className="form-control"
                          />
                          {errors.email && touched.email && (
                            <span className="input-feedback text-danger">
                              {errors.email}
                            </span>
                          )}
                        </div>
                        <div className="form-group col-md-6">
                          <label htmlFor="Phone">Phone</label>
                          <Field
                            name="phone"
                            type="text"
                            values={values.phone}
                            placeholder="123-456-7890"
                            autoComplete="off"
                            className="form-control"
                          />
                          {errors.phone && touched.phone && (
                            <span className="input-feedback text-danger">
                              {errors.phone}
                            </span>
                          )}
                        </div>
                      </div>
                      <div className="form-group">
                        <label htmlFor="AvatarUrl">Avatar Url</label>
                        <Field
                          name="avatarUrl"
                          type="text"
                          values={values.avatarUrl}
                          placeholder="http://"
                          autoComplete="off"
                          className="form-control"
                        />
                        {errors.avatarUrl && touched.avatarUrl && (
                          <span className="input-feedback text-danger">
                            {errors.avatarUrl}
                          </span>
                        )}
                      </div>
                      {this.state.hasLocation ? (
                        <LocationBasicForm />
                      ) : (
                        <button
                          onClick={this.toggleLocation}
                          className="btn btn-outline-secondary mb-3"
                        >
                          {"+ Add Location"}
                        </button>
                      )}
                      <div className="form-group">
                        <label htmlFor="Notes">Notes</label>
                        <Field
                          className="form-control"
                          name="Notes"
                          as="textarea"
                          rows={4}
                          defaultValue={""}
                        />
                      </div>
                      <button
                        type="submit"
                        className="btn btn-primary"
                        disabled={!isValid || isSubmitting}
                      >
                        Submit
                      </button>
                    </form>
                  </div>
                </div>
              </div>
            );
          }}
        </Formik>
      </React.Fragment>
    );
  }
}

AddTherapist.propTypes = {
  history: PropTypes.shape({
    push: PropTypes.func,
  }),
};

export default withRouter(AddTherapist);
