Imports limc.TypeSystem

Namespace Repository
    Public Class MethodRepository

        ' Contains all functions with their C variants
        Private ReadOnly Methods As New Dictionary(Of String, MethodSet)

        ' Register uncompiled method
        Public Sub RegisterMethod(Model As AST.FunctionConstruct, AssociatedType As TypeSystem.Type)
            If Not Methods.ContainsKey(Model.Name) Then
                Methods(Model.Name) = New MethodSet()
            End If
            Methods(Model.Name).RegisterVariant(New MethodVariantContainer(Model, AssociatedType))
        End Sub

        ' Register compiled method
        Public Sub RegisterMethod(Method As Lazy.Method)
            If Not Methods.ContainsKey(Method.Name) Then
                Methods(Method.Name) = New MethodSet()
            End If
            Methods(Method.Name).RegisterVariant(New MethodContainer(Method))
        End Sub

        ' General repository endpoint
        Public Function RetrieveMethods(Name As String, GenericTypes As IEnumerable(Of TypeSystem.Type)) As IEnumerable(Of Lazy.Method)
            If Methods.ContainsKey(Name) Then
                Return Methods(Name).RetrieveWithGenerics(GenericTypes)
            Else
                Return {}
            End If
        End Function

        ' Represent a set of functions with the same name
        Private Class MethodSet

            Private Variants As New List(Of IMethodContainer)

            Public Sub RegisterVariant(V As IMethodContainer)
                Variants.Add(V)
            End Sub

            Public Function RetrieveWithGenerics(GenericTypes As IEnumerable(Of TypeSystem.Type)) As IEnumerable(Of Lazy.Method)
                Dim Results As New List(Of Lazy.Method)

                For Each V In Variants
                    Dim RetrievedMethod As Lazy.Method = V.RetrieveMethod(GenericTypes)
                    If RetrievedMethod IsNot Nothing Then
                        Results.Add(RetrievedMethod)
                    End If
                Next

                Return Results
            End Function

        End Class

        ' Represent a function, contains all C subfunctions for each generic type combination
        Private Interface IMethodContainer
            Function RetrieveMethod(GenericTypes As IEnumerable(Of TypeSystem.Type)) As Lazy.Method
        End Interface

        Private Class MethodVariantContainer
            Implements IMethodContainer

            Private Model As AST.FunctionConstruct
            Private AssociatedType As TypeSystem.Type
            Private LazyMethods As New List(Of Lazy.Method)

            Public Sub New(Model As AST.FunctionConstruct, AssociatedType As TypeSystem.Type)
                Me.Model = Model
                Me.AssociatedType = AssociatedType
            End Sub

            Public Function RetrieveMethod(GenericTypes As IEnumerable(Of TypeSystem.Type)) As Lazy.Method Implements IMethodContainer.RetrieveMethod

                ' Count error
                If GenericTypes.Count <> Model.GenericArguments.Count Then
                    Return Nothing
                End If

                ' Get the matching lazy function
                Dim MatchingLazyMethod As Lazy.Method = LazyMethods.FirstOrDefault(Function(Fn) Fn.PassedGenericTypes.SequenceEqual(GenericTypes))

                ' If not found, create it
                If MatchingLazyMethod Is Nothing Then
                    Dim LazyFn As New Lazy.UserMethod(AssociatedType, Model, GenericTypes)
                    LazyMethods.Add(LazyFn)
                    Return LazyFn
                Else
                    Return MatchingLazyMethod
                End If

            End Function

        End Class

        Private Class MethodContainer
            Implements IMethodContainer

            Private Method As Lazy.Method

            Public Sub New(Method As Lazy.Method)
                Me.Method = Method
            End Sub

            Public Function RetrieveMethod(GenericTypes As IEnumerable(Of Type)) As Lazy.Method Implements IMethodContainer.RetrieveMethod

                ' Count error
                If GenericTypes.Count <> Method.PassedGenericTypes.Count Then
                    Return Nothing
                End If

                ' If not found, create it
                If Method.PassedGenericTypes.SequenceEqual(GenericTypes) Then
                    Return Method
                Else
                    Return Nothing
                End If

            End Function

        End Class

    End Class
End Namespace