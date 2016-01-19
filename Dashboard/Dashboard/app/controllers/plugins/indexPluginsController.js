(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .controller('indexPluginsController', ['$scope', 'FileUploader', '$http', indexPluginsController]);

    function indexPluginsController($scope, FileUploader, $http) {

        $scope.toogleMenu = function () {
            $("#wrapper").toggleClass("toggled");
        }
    }
})();
