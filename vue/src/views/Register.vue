<template>
<div id="main-content">
  <div id="register" class="text-center">
    <form class="form-register" @submit.prevent="register">
      <h1 class="h3 mb-3 font-weight-normal">Create Account</h1>
      <div class="alert alert-danger" role="alert" v-if="registrationErrors">
        {{ registrationErrorMsg }}
      </div>
      <div class="form-group">
        <input
          type="text"
          id="email"
          class="form-control"
          placeholder="Email"
          v-model="user.email"
        required
        autofocus />
      </div>
      <div class="form-group">
        <input
          type="text"
          id="username"
          class="form-control"
          placeholder="Username"
          v-model="user.username"
          required/>
      </div>
      <div class="form-group">
        <input
          type="password"
          id="password"
          class="form-control"
          placeholder="Password"
          v-model="user.password"
          required />
      </div>
      <div class="form-group">
        <input
          type="password"
          id="confirmPassword"
          class="form-control"
          placeholder="Confirm Password"
          v-model="user.confirmPassword"
          required />
      </div>
      <div class="form-group">
        <router-link :to="{ name: 'login' }">Have an account?</router-link>
      </div>
      <button class="btn btn-primary" type="submit">
        Create Account
      </button>
    </form>
  </div>
</div>
</template>

<script>
import authService from '../services/AuthService.js';
import ActivityService from '../services/ActivityService.js';

export default {
  name: 'register',
  data() {
    return {
      user: {
        email: '',
        username: '',
        password: '',
        confirmPassword: '',
        role: 'user',
      },
      activity: {
        logId: 0,
        userId: 0,
        activityName: '',
        time: '2000-01-01T00:00:00.000'
      },
      registrationErrors: false,
      registrationErrorMsg: 'There were problems registering this user.',
    };
  },
  methods: {
    register() {
      if (this.user.password != this.user.confirmPassword) {
        this.registrationErrors = true;
        this.registrationErrorMsg = 'Password & Confirm Password do not match.';
      } else {
        authService
          .register(this.user)
          .then((response) => {
            if (response.status == 201) {
              this.activity.activityName = 'account created';
              ActivityService.addActivityToLog(this.activity);
              this.$router.push({
                name: 'login',
                query: { registration: 'success' },
              });
            }
          })
          .catch((error) => {
            const response = error.response;
            this.registrationErrors = true;
            if (response.status === 400) {
              this.registrationErrorMsg = 'Bad Request: Validation Errors';
            }
          });
      }
    },
    clearErrors() {
      this.registrationErrors = false;
      this.registrationErrorMsg = 'There were problems registering this user.';
    },
  },
};
</script>

<style scoped>

#main-content{
    height: 100vh;
    width: 100%;
    padding-bottom: 1rem;
}

#register{
  margin-top: 3rem;
}

</style>
