# QQ登录设置

## 进入设置界面

集成QQ一键登录需要首选进入 SiteServer CMS 管理后台第三方登录设置：

![](./assets/qq/01.png)

点击QQ，进入详细设置界面：

![](./assets/qq/03.png)

现在我们需要获取 App ID 以及 App Key 值。

## 获取参数值

App ID 以及 App Key 值需要从 [QQ互联](https://open.qq.qq.com/) 获取，进入QQ互联，找到**应用管理**，点击**网站应用**：

![](./assets/qq/04.png)

点击右侧**查看**链接：

![](./assets/qq/05.png)

## 网站回调域

请确保应用详情页面中最底部的**网站回调域**为正式的域名地址（如果网站API为独立部署，请设置为API的访问域名） + “/api/SS.Login/auth/Qq/redirect”，否则QQ登录页面将报错。

如：`http://www.domain.com/api/SS.Login/auth/Qq/redirect`

![](./assets/qq/06.png)

如果网站回调域不正确，可以点击修改链接，进行修改。

## 测试

设置完成后进入 SiteServer CMS 后台第三方登录页面，点击**测试**链接，如果链接地址为QQ登录界面，说明设置成功。

![](./assets/qq/02.png)

注意：必须在正式域名下进行测试，否则可能无法成功。 