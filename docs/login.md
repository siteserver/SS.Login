# &lt;stl:login&gt; 登录标签

登录标签用于在页面中弹出登录框。

```html
<stl:login type="类型" redirectUrl="登录成功后的转向地址"></stl:login>
```

## 说明

&lt;stl:login&gt; 元素最终将解析为超链接a标签，所有a标签可用的属性均可以加到stl:login标签中。

可以把任何 STL 标签或者 HTML 标签（图片等）嵌套在 &lt;stl:login&gt; 元素内，用户点击嵌套的内容时页面将弹出登录框。

## 属性

所有属于HTML 标签&lt;a&gt;的属性均适用于&lt;stl:a&gt;标签，请参考：[HTML &lt;a&gt; 元素](http://docs.siteserver.cn/stl/#/reference_html/a)。

| 属性                                         | 说明                 |
| -------------------------------------------- | -------------------- |
| [type](login?id=type) | 类型             |
| [redirectUrl](login?id=redirectUrl)   | 登录成功后的转向地址             |

### type

type 属性用于设置登录类型，如果不设置，默认为不启用第三方登录。

type 属性有如下取值：

- `"default"` 默认，不启用第三方登录，点击后在页面中弹出登录框。
- `"weibo"` 点击后进入新浪微博登录界面，登录成功后返回。
- `"weixin"` 点击后进入微信登录界面，登录成功后返回。
- `"qq"` 点击后进入QQ登录界面，登录成功后返回。
- `"all"` 点击后在页面中弹出登录框，可以通过第三方账号一键登录，同时也能够通过用户名密码进行登录。

type 属性的详细用法请参考：[登录标签示例](/sample)。

### redirectUrl

设置登录成功后的转向地址，如果不设置，默认为返回当前页面。

```html
<!-- 登录成功后转入用户中心 -->
<stl:login redirectUrl="/home"></stl:login>
```

## 第三方登录标签调用

```
<stl:login type="weibo">微博登录</stl:login>
<stl:login type="weixin">微信登录</stl:login>
<stl:login type="qq">QQ登录</stl:login>
<stl:login type="all">一键登录</stl:login>
```