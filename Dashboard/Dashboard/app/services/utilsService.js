(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .factory('utilsService', utilsService);

    utilsService.$inject = ['$http'];

    function utilsService($http) {
        var service = {
            parseModelStateErrors: parseModelStateErrors
        };

        return service;

        function parseModelStateErrors(data) {
            var errors = [];
            for (var key in data.modelState) {
                if (data.modelState.hasOwnProperty(key)) {
                    for (var i = 0; i < data.modelState[key].length; i++) {
                        errors.push(data.modelState[key][i]);
                    }
                }
            }

            return errors;
        }
    }
})();