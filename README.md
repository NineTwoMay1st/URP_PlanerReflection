# URP_PlanerReflection

![image](https://user-images.githubusercontent.com/49144051/132651332-676bf0ce-6b5c-4f94-a248-e4b740e3c32c.png)

一个平面反射效果

原理：创建一个反射相机，其参数和主相机相同，其位置与主相机相对于反射物体对称（只适用与反射物体和y轴垂直的情况，例如地面），反射相机的画面渲染到一个
RenderTexture上。然后在反射物体的shader上采样这个Texture,并将其叠加到最终颜色上。

Tip：修改官方的unity.render-pipelines.universal Package，资源在https://github.com/NineTwoMay1st/URP_PlanerReflection_com.unity.render-pipelines.Universal。 其存储路径要对应工程Package/manifest.json对应条目的路径。
