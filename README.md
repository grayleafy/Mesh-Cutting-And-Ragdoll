# Mesh-Cutting-And-Ragdoll
模型切割和布娃娃效果的简单实现

# 模型切割的算法实现
先合并空间重叠的顶点，然后用切割三角形面片

根据不同的切割面（无线平面，圆形平面等）处理三角形面片的顶点，可将顶点分为三类：切割面正方向、切割面反方向、置疑。切割时需要维护切割得到的新顶点的构成的多边形的边

所有三角形面片都被切割处理后，使用并查集确定之前置疑的顶点分类

使用耳切法将多边形切割面转换为三角形集合

最后处理三角形片元数据（法线向量、贴图坐标、骨骼权重）得到最终解

# 效果演示

## 球体切割
![屏幕截图 2023-08-18 143848](https://github.com/grayleafy/Mesh-Cutting-And-Ragdoll/assets/86156654/8c37f9d5-db90-4e14-85c3-f6f9c79ab20a)
![屏幕截图 2023-08-18 143930](https://github.com/grayleafy/Mesh-Cutting-And-Ragdoll/assets/86156654/fd5f9ee7-8877-4eb7-aec9-384df06b89c0)
## 正方体切割
![屏幕截图 2023-08-18 143957](https://github.com/grayleafy/Mesh-Cutting-And-Ragdoll/assets/86156654/11f2232e-b2b4-4ae8-b4e2-48efa281052d)
![屏幕截图 2023-08-18 144035](https://github.com/grayleafy/Mesh-Cutting-And-Ragdoll/assets/86156654/2d38a684-06e5-4fae-a859-f652a2384515)
## 蒙皮网络切割
![屏幕截图 2023-08-18 144219](https://github.com/grayleafy/Mesh-Cutting-And-Ragdoll/assets/86156654/61882d59-0d83-4087-9b4e-13498bb375f7)
![屏幕截图 2023-08-18 144151](https://github.com/grayleafy/Mesh-Cutting-And-Ragdoll/assets/86156654/48cd7912-2c6e-4983-82d0-f79429a4e771)
## 布娃娃效果
![屏幕截图 2023-08-18 143749](https://github.com/grayleafy/Mesh-Cutting-And-Ragdoll/assets/86156654/cc959c7e-8c61-405c-a562-3a0edae9b077)
