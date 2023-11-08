!function(){var u;function e(t){return new EventSource(t,{withCredentials:!0})}function a(t){return t.trim().split(/\s+/)}function c(t){t=u.getAttributeValue(t,"hx-sse");if(t)for(var e=a(t),n=0;n<e.length;n++){var r=e[n].split(/:(.+)/);if("connect"===r[0])return r[1]}}function l(t){var t=u.getAttributeValue(t,"hx-sse"),e=[];if(t)for(var n=a(t),r=0;r<n.length;r++){var s=n[r].split(/:(.+)/);"swap"===s[0]&&e.push(s[1])}return e}function f(t){if(!u.bodyContains(t)){t=u.getInternalData(t).sseEventSource;if(null!=t)return t.close(),1}}function g(t,e){var n=[];return(u.hasAttribute(t,e)||u.hasAttribute(t,"hx-sse"))&&n.push(t),t.querySelectorAll("["+e+"], [data-"+e+"], [hx-sse], [data-hx-sse]").forEach(function(t){n.push(t)}),n}function h(e,n){u.withExtensions(e,function(t){n=t.transformResponse(n,null,e)});var t=u.getSwapSpecification(e),r=u.getTarget(e),s=u.makeSettleInfo(e);u.selectAndSwap(t.swapStyle,r,e,n,s),s.elts.forEach(function(t){t.classList&&t.classList.add(htmx.config.settlingClass),u.triggerEvent(t,"htmx:beforeSettle")}),0<t.settleDelay?setTimeout(i(s),t.settleDelay):i(s)()}function i(t){return function(){t.tasks.forEach(function(t){t.call()}),t.elts.forEach(function(t){t.classList&&t.classList.remove(htmx.config.settlingClass),u.triggerEvent(t,"htmx:afterSettle")})}}htmx.defineExtension("sse",{init:function(t){u=t,null==htmx.createEventSource&&(htmx.createEventSource=e)},onEvent:function(t,e){switch(t){case"htmx:beforeCleanupElement":var n=u.getInternalData(e.target);return void(n.sseEventSource&&n.sseEventSource.close());case"htmx:afterProcessNode":!function e(i,n){if(null==i)return null;var t=u.getInternalData(i);var r=u.getAttributeValue(i,"sse-connect");if(null==r){var s=c(i);if(!s)return null;r=s}var o=htmx.createEventSource(r);t.sseEventSource=o;o.onerror=function(t){u.triggerErrorEvent(i,"htmx:sseError",{error:t,source:o}),f(i)||o.readyState===EventSource.CLOSED&&(n=n||0,t=Math.random()*(2^n)*500,window.setTimeout(function(){e(i,Math.min(7,n+1))},t))};o.onopen=function(t){u.triggerEvent(i,"htmx:sseOpen",{source:o})};g(i,"sse-swap").forEach(function(e){var t,n=u.getAttributeValue(e,"sse-swap");t=n?n.split(","):l(e);for(var r=0;r<t.length;r++){var s=t[r].trim(),a=function(t){f(i)?o.removeEventListener(s,a):(h(e,t.data),u.triggerEvent(i,"htmx:sseMessage",t))};u.getInternalData(i).sseEventListener=a,o.addEventListener(s,a)}});g(i,"hx-trigger").forEach(function(e){var n,r=u.getAttributeValue(e,"hx-trigger");null!=r&&"sse:"==r.slice(0,4)&&(n=function(t){f(i)?o.removeEventListener(r,n):(htmx.trigger(e,r,t),htmx.trigger(e,"htmx:sseMessage",t))},u.getInternalData(i).sseEventListener=n,o.addEventListener(r.slice(4),n))})}(e.target)}}})}();