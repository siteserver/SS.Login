# 与&lt;stl:if&gt;标签配合使用

如果希望显示登录状态并根据登录状态显示不同的内容，我们需要与&lt;stl:if&gt;标签配合使用。

用户登录状态的判断类型为`IsUserLoggin`，关于&lt;stl:if&gt;标签的详细说明，请参考：[STL &lt;if&gt; 元素](http://docs.siteserver.cn/stl/#/if/)。

## 示例

```html
<stl:if type="IsUserLoggin">
    <stl:yes>
        <div class="user">
            您好，<b>{user.DisplayName}</b>
            <a href="/home">用户中心</a>
            <a href="/shopping/cart.html">购物车</a>
            <a href="/shopping/orders.html">我的订单</a>
            <stl:logout class="logout">退出</stl:logout>
        </div>
    </stl:yes>
    <stl:no>
        <div class="user">
            <stl:login style="color:#797979;">登录</stl:login>
            <stl:register style="color:#797979;">注册</stl:register>
        </div>
        <div class="user_other">
            <stl:login type="weibo" class="weibo">微博登录</stl:login>
            <stl:login type="qq" class="qq">QQ登录</stl:login>
            <stl:login type="weixin" class="weixin">微信登录</stl:login>
        </div>
    </stl:no>
</stl:if>
```

以上代码实现的功能为判断用户是否登录，如果已登录显示用户姓名以及链接，如果未登录显示登录、注册以及第三方登录链接。

最后效果（未登录）：

![](./assets/if/01.png)

最后效果（已登录）：

![](./assets/if/02.png)