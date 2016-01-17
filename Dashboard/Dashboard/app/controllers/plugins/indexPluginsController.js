(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .controller('indexPluginsController', ['$scope', 'FileUploader', indexPluginsController]);

    function indexPluginsController($scope, FileUploader) {

        $scope.uploader = new FileUploader({
            url: 'api/plugins/upload'
        });

        $scope.uploader.filters.push({
            name: 'fileCountFilter',
            fn: function (item, options) {
                return this.queue.length < 2;
            }
        });

        activate();

        $scope.toogleMenu = function () {
            $("#wrapper").toggleClass("toggled");
        }

        function activate() { }
    }
})();
