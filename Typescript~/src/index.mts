import { arrayUtils } from "./array-utils.mjs";
import { enumfield, listproperty, mapleclass, property } from "./js-utils.mjs";
import { MonoBehaviorJs } from "./mono-behavior-js.mjs";
let global: any
global = global ?? globalThis ?? (function (this: any) { return this; }());

let maple = global.maple = global.maple || {};

maple.MonoBehaviorJs = MonoBehaviorJs;
maple.arrayUtils = arrayUtils;
global.property = property;
global.mapleclass = mapleclass;
global.listProperties = listproperty;
global.enumfield = enumfield;