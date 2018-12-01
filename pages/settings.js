var data = {
  pageLoad: false,
  pageType: 'list',
  pageAlert: null,
  config: null,
  urlWeibo: null,
  urlWeixin: null,
  urlQq: null,
  isWeibo: null,
  weiboAppKey: null,
  weiboAppSecret: null,
  isWeixin: null,
  weixinAppId: null,
  weixinAppSecret: null,
  isQq: null,
  qqAppId: null,
  qqAppKey: null
};

var methods = {
  submit: function () {
    var $this = this;

    var payload = {};
    if (this.pageType === 'weibo') {
      payload.isWeibo = this.isWeibo;
      payload.weiboAppKey = this.weiboAppKey;
      payload.weiboAppSecret = this.weiboAppSecret;
    } else if (this.pageType === 'weixin') {
      payload.isWeixin = this.isWeixin;
      payload.weixinAppId = this.weixinAppId;
      payload.weixinAppSecret = this.weixinAppSecret;
    } else if (this.pageType === 'qq') {
      payload.isQq = this.isQq;
      payload.qqAppId = this.qqAppId;
      payload.qqAppKey = this.qqAppKey;
    }

    utils.loading(true);
    $api.post('settings', payload).then(function (response) {
      var res = response.data;

      $this.config = res.value;
      $this.urlWeibo = res.urlWeibo;
      $this.urlWeixin = res.urlWeixin;
      $this.urlQq = res.urlQq;

      $this.isWeibo = $this.config.isWeibo;
      $this.weiboAppKey = $this.config.weiboAppKey;
      $this.weiboAppSecret = $this.config.weiboAppSecret;
      $this.isWeixin = $this.config.isWeixin;
      $this.weixinAppId = $this.config.weixinAppId;
      $this.weixinAppSecret = $this.config.weixinAppSecret;
      $this.isQq = $this.config.isQq;
      $this.qqAppId = $this.config.qqAppId;
      $this.qqAppKey = $this.config.qqAppKey;

      alert({
        toast: true,
        type: 'success',
        title: "设置保存成功",
        showConfirmButton: false,
        timer: 2000
      });
      $this.pageType = 'list';
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      utils.loading(false);
    });
  },

  btnSubmitClick: function () {
    var $this = this;
    this.pageAlert = null;

    this.$validator.validate().then(function (result) {
      if (result) {
        $this.submit();
      }
    });
  }
};

var $vue = new Vue({
  el: "#main",
  data: data,
  methods: methods,
  created: function () {
    var $this = this;

    $api.get('settings').then(function (response) {
      var res = response.data;

      $this.config = res.value;
      $this.urlWeibo = res.urlWeibo;
      $this.urlWeixin = res.urlWeixin;
      $this.urlQq = res.urlQq;

      $this.isWeibo = $this.config.isWeibo;
      $this.weiboAppKey = $this.config.weiboAppKey;
      $this.weiboAppSecret = $this.config.weiboAppSecret;
      $this.isWeixin = $this.config.isWeixin;
      $this.weixinAppId = $this.config.weixinAppId;
      $this.weixinAppSecret = $this.config.weixinAppSecret;
      $this.isQq = $this.config.isQq;
      $this.qqAppId = $this.config.qqAppId;
      $this.qqAppKey = $this.config.qqAppKey;
    }).catch(function (error) {
      $this.pageAlert = utils.getPageAlert(error);
    }).then(function () {
      $this.pageLoad = true;
    });
  }
});