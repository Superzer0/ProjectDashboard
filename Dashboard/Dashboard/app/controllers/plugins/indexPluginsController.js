(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .controller('indexPluginsController', indexPluginsController);

    indexPluginsController.$inject = ['$scope']; 

    function indexPluginsController($scope) {

        activate();

        function activate() { }
    }
})();
