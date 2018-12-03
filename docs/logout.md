# &lt;stl:logout&gt; 退出标签

退出标签用于退出登录状态。

```html
<stl:logout redirectUrl="退出后的转向地址"></stl:logout>
```

## 说明

&lt;stl:logout&gt; 元素最终将解析为超链接a标签，所有a标签可用的属性均可以加到stl:logout标签中。

可以把任何 STL 标签或者 HTML 标签（图片等）嵌套在 &lt;stl:logout&gt; 元素内。

## 属性

所有属于HTML 标签&lt;a&gt;的属性均适用于&lt;stl:a&gt;标签，请参考：[HTML &lt;a&gt; 元素](http://docs.siteserver.cn/stl/#/reference_html/a)。

| 属性                                         | 说明                 |
| -------------------------------------------- | -------------------- |
| [redirectUrl](logout?id=redirectUrl)   | 退出成功后的转向地址             |

### redirectUrl

设置退出成功后的转向地址，如果不设置，默认为返回当前页面。

```html
<!-- 退出成功后转入指定页面 -->
<stl:logout redirectUrl="/success.html"></stl:logout>
```