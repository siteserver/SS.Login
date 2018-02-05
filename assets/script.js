  var authModalId = "#ss-auth";

  Vue.use(VeeValidate);
  var $authVue = new Vue({
    el: authModalId,
    data: {
      apiUrlRegister: authData.apiUrlRegister,
      apiUrlLogin: authData.apiUrlLogin,
      apiUrlLogout: authData.apiUrlLogout,
      apiUrlWeixin: authData.apiUrlWeixin,
      apiUrlWeibo: authData.apiUrlWeibo,
      apiUrlQq: authData.apiUrlQq,
      registerSuccessMessage: authData.registerSuccessMessage,
      loginRedirectUrl: authData.loginRedirectUrl,
      logoutRedirectUrl: authData.logoutRedirectUrl,

      form: '',
      title: '',
      register: {
        userName: '',
        displayName: '',
        email: '',
        mobile: '',
        password: ''
      },
      login: {
        account: '',
        password: ''
      },
      errorMessage: ''
    },
    methods: {
      registerSubmit: function () {
        var $this = this;
        this.$validator.validateAll('register_form').then((result) => {
          if (result) {
            $this.apiRegister();
          }
        });
      },
      apiRegister: function () {
        var $this = this;
        axios.post($this.apiUrlRegister, this.register)
          .then(function (response) {
            $this.form = 'registerSuccess';
          })
          .catch(function (error) {
            $this.errorMessage = error.response.data.message;
          });
      },
      loginSubmit: function () {
        var $this = this;
        this.$validator.validateAll('login_form').then((result) => {
          if (result) {
            $this.apiLogin();
          }
        });
      },
      apiLogin: function () {
        var $this = this;
        axios.post($this.apiUrlLogin, this.login)
          .then(function (response) {
            location.href = $this.loginRedirectUrl;
          })
          .catch(function (error) {
            $this.errorMessage = error.response.data.message;
          });
      },
      logout: function () {
        var $this = this;
        axios.post($this.apiUrlLogout)
          .then(function (response) {
            location.href = $this.logoutRedirectUrl;
          })
          .catch(function (error) {
            console.log(error.response.data.message);
          });
      },
      openModal: function () {
        var $this = this;

        $("body").append($("<div id='lean_overlay'></div>"));
        $("#lean_overlay").click(function () {
          $this.closeModal();
        });
        $("#lean_overlay").css({
          "display": "block",
          opacity: 0
        });
        $("#lean_overlay").fadeTo(200, 0.5);

        var modal_height = $(authModalId).outerHeight();
        var modal_width = $(authModalId).outerWidth();

        $(authModalId).css({
          "display": "block",
          "position": "fixed",
          "opacity": 0,
          "z-index": 11000,
          "left": 50 + "%",
          "margin-left": -(modal_width / 2) + "px",
          "top": "100px"
        });
        $(authModalId).fadeTo(200, 1);
      },
      closeModal: function () {
        $("#lean_overlay").fadeOut(200);
        this.form = '';
      },
      openLoginModal: function () {
        this.form = 'login';
        this.title = '用户登录';
        this.openModal();
      },
      openSocialModal: function () {
        this.form = 'social';
        this.title = '用户登录';
        this.openModal();
      },
      openRegisterModal: function () {
        this.form = 'register';
        this.title = '用户注册';
        this.openModal();
      }
    }
  });