// simpleControls.js
(function() {
    "use strict";

    angular.module("simpleControls", [])
        .directive("wait-cursor", waitCursor);

    function waitCursor() {
        return {
            templateUrl: "/views/waitCursor.html"
        };
    }

})();