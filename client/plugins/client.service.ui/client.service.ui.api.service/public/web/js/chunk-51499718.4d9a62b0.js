(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-51499718"],{"71a7":function(e,t,r){"use strict";r("fffc")},9217:function(e,t,r){var n=r("24fb");t=n(!1),t.push([e.i,".proxy-wrap[data-v-6d88cff7]{padding:2rem}.alert[data-v-6d88cff7]{margin-bottom:1rem}.form[data-v-6d88cff7]{border:1px solid #eee;padding:2rem;border-radius:.4rem}@media screen and (max-width:768px){.el-col[data-v-6d88cff7]{margin-top:.6rem}}",""]),e.exports=t},da23:function(e,t,r){"use strict";r.r(t);r("b0c0");var n=r("7a23"),c={class:"proxy-wrap"},o={class:"title t-c"},a={class:"form"},l={class:"w-100"},u={class:"w-100"},d={class:"w-100 t-c"},i=Object(n["createTextVNode"])("确 定"),f=Object(n["createTextVNode"])("配置插件"),b={class:"w-100"};function s(e,t,r,s,m,O){var p=Object(n["resolveComponent"])("el-alert"),j=Object(n["resolveComponent"])("el-input"),w=Object(n["resolveComponent"])("el-form-item"),N=Object(n["resolveComponent"])("el-col"),V=Object(n["resolveComponent"])("el-option"),h=Object(n["resolveComponent"])("el-select"),x=Object(n["resolveComponent"])("el-row"),C=Object(n["resolveComponent"])("el-checkbox"),v=Object(n["resolveComponent"])("el-tooltip"),g=Object(n["resolveComponent"])("el-button"),P=Object(n["resolveComponent"])("ConfigureModal"),y=Object(n["resolveComponent"])("el-form");return Object(n["openBlock"])(),Object(n["createElementBlock"])("div",c,[Object(n["createElementVNode"])("h3",o,Object(n["toDisplayString"])(e.$route.meta.name),1),Object(n["createVNode"])(p,{class:"alert",type:"warning","show-icon":"",closable:!1,title:"http1.1代理，如果服务端开启，则也可以代理到服务端，在TCP转发配置被访问权限"}),Object(n["createElementVNode"])("div",a,[Object(n["createVNode"])(y,{ref:"formDom",model:s.state.form,rules:s.state.rules,"label-width":"80px"},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(w,{label:"","label-width":"0"},{default:Object(n["withCtx"])((function(){return[Object(n["createElementVNode"])("div",l,[Object(n["createVNode"])(x,{gutter:10},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(N,{xs:24,sm:6,md:6,lg:6,xl:6},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(w,{label:"监听端口",prop:"Port"},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(j,{modelValue:s.state.form.Port,"onUpdate:modelValue":t[0]||(t[0]=function(e){return s.state.form.Port=e})},null,8,["modelValue"])]})),_:1})]})),_:1}),Object(n["createVNode"])(N,{xs:24,sm:6,md:6,lg:6,xl:6},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(w,{label:"通信通道",prop:"TunnelType"},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(h,{modelValue:s.state.form.TunnelType,"onUpdate:modelValue":t[1]||(t[1]=function(e){return s.state.form.TunnelType=e}),placeholder:"选择类型"},{default:Object(n["withCtx"])((function(){return[(Object(n["openBlock"])(!0),Object(n["createElementBlock"])(n["Fragment"],null,Object(n["renderList"])(s.shareData.tunnelTypes,(function(e,t){return Object(n["openBlock"])(),Object(n["createBlock"])(V,{key:t,label:e,value:t},null,8,["label","value"])})),128))]})),_:1},8,["modelValue"])]})),_:1})]})),_:1}),Object(n["createVNode"])(N,{xs:24,sm:6,md:6,lg:6,xl:6},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(w,{label:"目标端",prop:"Name"},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(h,{modelValue:s.state.form.Name,"onUpdate:modelValue":t[2]||(t[2]=function(e){return s.state.form.Name=e}),placeholder:"选择目标"},{default:Object(n["withCtx"])((function(){return[(Object(n["openBlock"])(!0),Object(n["createElementBlock"])(n["Fragment"],null,Object(n["renderList"])(s.targets,(function(e,t){return Object(n["openBlock"])(),Object(n["createBlock"])(V,{key:t,label:e.label,value:e.Name},null,8,["label","value"])})),128))]})),_:1},8,["modelValue"])]})),_:1})]})),_:1})]})),_:1})])]})),_:1}),Object(n["createVNode"])(w,{label:"","label-width":"0"},{default:Object(n["withCtx"])((function(){return[Object(n["createElementVNode"])("div",u,[Object(n["createVNode"])(x,{gutter:10},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(N,{xs:12,sm:6,md:6,lg:6,xl:6},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(w,{"label-width":"0",prop:"Listening"},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(C,{modelValue:s.state.form.Listening,"onUpdate:modelValue":t[3]||(t[3]=function(e){return s.state.form.Listening=e}),label:"开启端口监听"},null,8,["modelValue"])]})),_:1})]})),_:1}),Object(n["createVNode"])(N,{xs:12,sm:6,md:6,lg:6,xl:6},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(w,{"label-width":"0",prop:"IsPac"},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(v,{class:"box-item",effect:"dark",content:"勾选则设置系统代理，不勾选则需要自己设置",placement:"top-start"},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(C,{modelValue:s.state.form.IsPac,"onUpdate:modelValue":t[4]||(t[4]=function(e){return s.state.form.IsPac=e}),label:"设置系统代理"},null,8,["modelValue"])]})),_:1})]})),_:1})]})),_:1}),Object(n["createVNode"])(N,{xs:12,sm:6,md:6,lg:6,xl:6},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(w,{"label-width":"0",prop:"IsCustomPac"},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(v,{class:"box-item",effect:"dark",content:"自定义pac还是使用预制的pac规则",placement:"top-start"},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(C,{modelValue:s.state.form.IsCustomPac,"onUpdate:modelValue":t[5]||(t[5]=function(e){return s.state.form.IsCustomPac=e}),label:"自定义pac"},null,8,["modelValue"])]})),_:1})]})),_:1})]})),_:1})]})),_:1})])]})),_:1}),Object(n["createVNode"])(w,{"label-width":"0"},{default:Object(n["withCtx"])((function(){return[Object(n["createElementVNode"])("div",d,[Object(n["createVNode"])(g,{type:"primary",loading:s.state.loading,onClick:s.handleSubmit,class:"m-r-1"},{default:Object(n["withCtx"])((function(){return[i]})),_:1},8,["loading","onClick"]),Object(n["createVNode"])(P,{className:"TcpForwardClientConfigure"},{default:Object(n["withCtx"])((function(){return[Object(n["createVNode"])(g,null,{default:Object(n["withCtx"])((function(){return[f]})),_:1})]})),_:1})])]})),_:1}),Object(n["createVNode"])(w,{"label-width":"0",class:"hidden-xs-only"},{default:Object(n["withCtx"])((function(){return[Object(n["createElementVNode"])("div",b,[Object(n["createVNode"])(j,{modelValue:s.state.form.Pac,"onUpdate:modelValue":t[6]||(t[6]=function(e){return s.state.form.Pac=e}),rows:22,type:"textarea",placeholder:"写pac内容",resize:"none"},null,8,["modelValue"])])]})),_:1})]})),_:1},8,["model","rules"])])])}r("d3b7"),r("25f0"),r("99af"),r("d81d"),r("a9e3"),r("e9c4");var m=r("a1e9"),O=r("f8aa"),p=r("5c40"),j=r("3ef4"),w=r("3fd2"),N=r("8286"),V=r("49f5"),h={components:{ConfigureModal:V["a"]},setup:function(){var e=Object(w["a"])(),t=Object(N["a"])(),r=function(){Object(O["e"])().then((function(e){var t=e[0]||{ID:0,Port:5412,ForwardType:2,TunnelType:"8",AliveType:2,Name:"",Listening:!1,Pac:"",IsPac:!1,IsCustomPac:!1};o.form.ID=t.ID,o.form.Port=t.Port,o.form.TunnelType=t.TunnelType.toString(),o.form.AliveType=t.AliveType,o.form.Name=t.Name,o.form.Listening=t.Listening,o.form.Pac=t.Pac,o.form.IsPac=t.IsPac,o.form.IsCustomPac=t.IsCustomPac,n()}))},n=function(){Object(O["f"])().then((function(e){o.form.Pac=e}))};Object(p["rb"])((function(){r()}));var c=Object(m["c"])((function(){return[{Name:"",label:"服务器"}].concat(e.clients.map((function(e){return{Name:e.Name,label:e.Name}})))})),o=Object(m["p"])({loading:!1,pac:"",form:{ID:0,Port:5413,ForwardType:2,TunnelType:"8",AliveType:2,Name:"",Listening:!1,Pac:"",IsPac:!1,IsCustomPac:!1},rules:{Port:[{required:!0,message:"必填",trigger:"blur"},{type:"number",min:1,max:65535,message:"数字 1-65535",trigger:"blur",transform:function(e){return Number(e)}}]}}),a=Object(m["r"])(null),l=function(){a.value.validate((function(e){if(!e)return!1;o.loading=!0;var t=JSON.parse(JSON.stringify(o.form));t.Port=Number(t.Port),t.TunnelType=Number(t.TunnelType),Object(O["c"])(t).then((function(){o.loading=!1,j["a"].success("操作成功！"),r()})).catch((function(e){o.loading=!1}))}))};return{targets:c,shareData:t,state:o,formDom:a,handleSubmit:l}}},x=(r("71a7"),r("6b0d")),C=r.n(x);const v=C()(h,[["render",s],["__scopeId","data-v-6d88cff7"]]);t["default"]=v},f8aa:function(e,t,r){"use strict";r.d(t,"d",(function(){return c})),r.d(t,"e",(function(){return o})),r.d(t,"f",(function(){return a})),r.d(t,"l",(function(){return l})),r.d(t,"n",(function(){return u})),r.d(t,"c",(function(){return d})),r.d(t,"j",(function(){return i})),r.d(t,"b",(function(){return f})),r.d(t,"i",(function(){return b})),r.d(t,"h",(function(){return s})),r.d(t,"g",(function(){return m})),r.d(t,"a",(function(){return O})),r.d(t,"m",(function(){return p})),r.d(t,"o",(function(){return j})),r.d(t,"k",(function(){return w}));r("e9c4");var n=r("97af"),c=function(){return Object(n["c"])("tcpforward/list")},o=function(){return Object(n["c"])("tcpforward/listproxy")},a=function(){return Object(n["c"])("tcpforward/GetPac")},l=function(e){return Object(n["c"])("tcpforward/start",{ID:e})},u=function(e){return Object(n["c"])("tcpforward/stop",{ID:e})},d=function(e){return Object(n["c"])("tcpforward/AddListen",{ID:e.ID,Content:JSON.stringify(e)})},i=function(e){return Object(n["c"])("tcpforward/RemoveListen",{ID:e})},f=function(e){return Object(n["c"])("tcpforward/AddForward",{ID:e.Forward.ID,Content:JSON.stringify(e)})},b=function(e,t){return Object(n["c"])("tcpforward/RemoveForward",{ID:t,Content:JSON.stringify({ListenID:e,ForwardID:t})})},s=function(){return Object(n["c"])("tcpforward/ServerPorts")},m=function(){return Object(n["c"])("tcpforward/ServerForwards")},O=function(e){return Object(n["c"])("tcpforward/AddServerForward",e)},p=function(e){return Object(n["c"])("tcpforward/StartServerForward",e)},j=function(e){return Object(n["c"])("tcpforward/StopServerForward",e)},w=function(e){return Object(n["c"])("tcpforward/RemoveServerForward",e)}},fffc:function(e,t,r){var n=r("9217");n.__esModule&&(n=n.default),"string"===typeof n&&(n=[[e.i,n,""]]),n.locals&&(e.exports=n.locals);var c=r("499e").default;c("4babb41a",n,!0,{sourceMap:!1,shadowMode:!1})}}]);