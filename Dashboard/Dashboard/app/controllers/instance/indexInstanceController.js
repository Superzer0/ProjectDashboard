(function () {
    'use strict';

    angular
        .module('angularAdmin')
        .controller('indexInstanceController', indexInstanceController);

    indexInstanceController.$inject = ['$scope']; 

    function indexInstanceController($scope) {
        $scope.title = 'indexInstanceController';

        activate();

        function activate() { }
    }
})();
