using System;
using System.Collections.Generic;
using Xunit;
using MonetateLibraries;


namespace CmsTransformTests
{
    public class CmsTransformUnitTests
    {
        [Fact]
        public void TestEmptyCmsContentEmptyList()
        {
            string cmsContent = "{}";
            var transforms = new List<Dictionary<string, object>>();
            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            Assert.Equal(cmsContent, transformedContent);
            Assert.Empty(errors);
        }

        [Fact]
        public void TestNonEmptyCmsContentEmptyList()
        {
            string cmsContent = "{\"test\":\"value\"}";
            var transforms = new List<Dictionary<string, object>>();
            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            Assert.Equal(cmsContent, transformedContent);
            Assert.Empty(errors);
        }

        [Fact]
        public void TestInvalidTransformAction() {
            string cmsContent = "{\"test\":\"value\"}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "invalid_action");
            transform.Add("jsonPath", "test");
            transform.Add("value", "replaced");
            transforms.Add(transform);
            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":\"value\"}";
            Assert.Equal(expectedContent, transformedContent);
            Assert.Single(errors);
        }

        [Fact]
        public void TestReplace()
        {
            string cmsContent = "{\"test\":\"value\"}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "replace");
            transform.Add("jsonPath", "test");
            transform.Add("value", "replaced");
            transforms.Add(transform);
            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":\"replaced\"}";
            Assert.Equal(expectedContent, transformedContent);
            Assert.Empty(errors);
        }

        [Fact]
        public void TestMoveToIndex()
        {
            string cmsContent = "{\"test\":[\"first\",\"second\",\"third\"]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "moveToIndex");
            transform.Add("jsonPath", "test[2]");
            transform.Add("value", 0);
            transforms.Add(transform);
            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[\"third\",\"first\",\"second\"]}";
            Assert.Equal(expectedContent, transformedContent);
            Assert.Empty(errors);
        }

        [Fact]
        public void TestMoveToIndexInvalidValue()
        {
            string cmsContent = "{\"test\":[\"first\",\"second\",\"third\"]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "moveToIndex");
            transform.Add("jsonPath", "test[2]");
            transform.Add("value", "invalid");
            transforms.Add(transform);
            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            Assert.Equal(cmsContent, transformedContent);
            Assert.Single(errors);
            Assert.Equal("'invalid' is not an integer", errors[0]);
        }

        [Fact]
        public void TestMoveToIndexInvalidOrdinalPosition() {
            string cmsContent = "{\"test\":[\"first\",\"second\",\"third\"]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "moveToIndex");
            transform.Add("jsonPath", "test");
            transform.Add("value", 8);
            transforms.Add(transform);
            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            Assert.Equal(cmsContent, transformedContent);
            Assert.Single(errors);
        }

        [Fact]
        public void TestJsonPathNotFound()
        {
            string cmsContent = "{\"test\":\"value\"}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "replace");
            transform.Add("jsonPath", "invalidJsonPath");
            transform.Add("value", "replaced");
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);

            List<string> expectedErrors = new List<string>();
            expectedErrors.Add("Node at 'invalidJsonPath' does not exist.");

            Assert.Equal(cmsContent, transformedContent);
            Assert.Equal(expectedErrors, errors);
        }

        [Fact]
        public void TestInsertAfter() {
            string cmsContent = "{\"test\":[\"first\",\"second\",\"third\"]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "insertAfter");
            transform.Add("jsonPath", "test[1]");
            transform.Add("value", "newValue");
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[\"first\",\"second\",\"newValue\",\"third\"]}";
            Assert.Equal(expectedContent, transformedContent);
            Assert.Empty(errors);
        }

        [Fact]
        public void TestInsertAfterOutOfBounds() {
            string cmsContent = "{\"test\":[\"first\",\"second\",\"third\"]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "insertAfter");
            transform.Add("jsonPath", "test[3]");
            transform.Add("value", "newValue");
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[\"first\",\"second\",\"third\"]}";
            Assert.Equal(expectedContent, transformedContent);
            Assert.Single(errors);
        }

        [Fact]
        public void TestInsertAfterInvalidPath() {
            string cmsContent = "{\"test\":[\"first\",\"second\",\"third\"]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "insertAfter");
            transform.Add("jsonPath", "invalid");
            transform.Add("value", "newValue");
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[\"first\",\"second\",\"third\"]}";
            Assert.Equal(expectedContent, transformedContent);
            Assert.Single(errors);
        }

        [Fact]
        public void TestInsertAfterTest() {
            string cmsContent = "{\"test\":[\"first\",\"second\",\"third\"]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "insertAfter");
            transform.Add("jsonPath", "test");
            transform.Add("value", "newValue");
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[\"first\",\"second\",\"third\"]}";
            Assert.Equal(expectedContent, transformedContent);
            Assert.Single(errors);
        }

        [Fact]
        public void TestInsertAfterEmptyPath() {
            string cmsContent = "{\"test\":[\"first\",\"second\",\"third\"]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "insertAfter");
            transform.Add("jsonPath", "");
            transform.Add("value", "newValue");
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[\"first\",\"second\",\"third\"]}";
            Assert.Equal(expectedContent, transformedContent);
            Assert.Single(errors);
        }

        [Fact]
        public void TestInsertAfterTestMultiple() {
            string cmsContent = "{\"test\":[\"first\",\"second\",\"third\"], \"orange\":[\"first\",\"second\",\"third\"]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "insertAfter");
            transform.Add("jsonPath", "orange[2]");
            transform.Add("value", "newValue");
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[\"first\",\"second\",\"third\"],\"orange\":[\"first\",\"second\",\"third\",\"newValue\"]}";
            Assert.Equal(expectedContent, transformedContent);
            Assert.Empty(errors);
        }

        [Fact]
        public void TestAddToList() {
            string cmsContent = "{\"test\":[{\"foo\":\"bar\"}]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "addToList");
            transform.Add("jsonPath", "test");
            var newValue = new Dictionary<string, string>();
            newValue.Add("spam","ham");
            transform.Add("value", newValue);
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[{\"foo\":\"bar\"},{\"spam\":\"ham\"}]}";
            Assert.Equal(expectedContent, transformedContent);
            Assert.Empty(errors);
        }

        [Fact]
        public void TestAddToListMultipleLists() {
            string cmsContent = "{\"test\":[{\"foo\":\"bar\"},{\"fiz\":\"baz\"}],\"spam\":[{\"ham\":\"eggs\"}]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "addToList");
            transform.Add("jsonPath", "test");
            var newValue = new Dictionary<string, string>();
            newValue.Add("orange","blue");
            transform.Add("value", newValue);
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[{\"foo\":\"bar\"},{\"fiz\":\"baz\"},{\"orange\":\"blue\"}],\"spam\":[{\"ham\":\"eggs\"}]}";
            Assert.Equal(expectedContent, transformedContent);
            Assert.Empty(errors);
        }

        [Fact]
        public void TestAddToListInvalidPath() {
            string cmsContent = "{\"test\":[{\"foo\":\"bar\"}]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "addToList");
            transform.Add("jsonPath", "goo");
            var newValue = new Dictionary<string, string>();
            newValue.Add("spam","ham");
            transform.Add("value", newValue);
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[{\"foo\":\"bar\"}]}";
            Assert.Equal(expectedContent, transformedContent);
            Assert.Single(errors);
        }

        [Fact]
        public void TestAddToObject() {
            string cmsContent = "{\"test\":[{\"foo\":\"bar\"},{\"fiz\":\"baz\"}]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "addToObject");
            transform.Add("jsonPath", "test[1]");
            transform.Add("key", "new_key");
            transform.Add("value", "newValue");
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[{\"foo\":\"bar\"},{\"fiz\":\"baz\",\"new_key\":\"newValue\"}]}";
            Assert.Equal(expectedContent, transformedContent);
        }

        [Fact]
        public void TestAddToObjectInvalidKeyStructure() {
            string cmsContent = "{\"test\":[{\"foo\":\"bar\"},{\"fiz\":\"baz\"}]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "addToObject");
            transform.Add("jsonPath", "test[1]");
            var key = new Dictionary<string, string>();
            key.Add("key", "some_key_name");
            transform.Add("key", key);
            transform.Add("value", "newValue");
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[{\"foo\":\"bar\"},{\"fiz\":\"baz\"}]}";
            Assert.Equal(expectedContent, transformedContent);
            Assert.Single(errors);
        }

        [Fact]
        public void TestAddToObjectDict() {
            string cmsContent = "{\"test\":[{\"foo\":\"bar\"},{\"fiz\":\"baz\"}]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "addToObject");
            transform.Add("jsonPath", "test[1]");
            var val = new Dictionary<string, string>();
            val.Add("spam", "ham");
            transform.Add("key", "new_key");
            transform.Add("value", val);
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[{\"foo\":\"bar\"},{\"fiz\":\"baz\",\"new_key\":{\"spam\":\"ham\"}}]}";
            Assert.Equal(expectedContent, transformedContent);
        }

        [Fact]
        public void TestRemove() {
            string cmsContent = "{\"test\":[\"first\",\"second\",\"third\"]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "remove");
            transform.Add("jsonPath", "test[1]");
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[\"first\",\"third\"]}";
            Assert.Equal(expectedContent, transformedContent);
        }

        [Fact]
        public void TestRemoveExecutesLast() {
            string cmsContent = "{\"test\":[{\"foo\":\"bar\"},{\"fiz\":\"baz\"}]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "remove");
            transform.Add("jsonPath", "test[1]");

            var transform2 = new Dictionary<string, object>();
            transform2.Add("method", "insertAfter");
            transform2.Add("jsonPath", "test[0]");
            var entry = new Dictionary<string, object>();
            entry.Add("this", "shouldNotBeInResult");
            transform2.Add("value", entry);
            transforms.Add(transform);
            transforms.Add(transform2);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[{\"foo\":\"bar\"},{\"fiz\":\"baz\"}]}";
            Assert.Equal(expectedContent, transformedContent);
        }

        [Fact]
        public void TestRemoveDict() {
            string cmsContent = "{\"test\":[{\"foo\":\"bar\"},{\"fiz\":\"baz\"}]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "remove");
            transform.Add("jsonPath", "test[0]");
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[{\"fiz\":\"baz\"}]}";
            Assert.Equal(expectedContent, transformedContent);
        }

        [Fact]
        public void TestRemoveSubcomponents() {
            string cmsContent = "{\"test\":[{\"foo\":\"1\",\"bar\":{\"subcomponent\":{\"item\":[{\"spam\":\"ham\"},{\"eggs\":\"yum\"}]}}},{\"fiz\":\"baz\"}]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "remove");
            transform.Add("jsonPath", "test[0].bar.subcomponent.item[0]");
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[{\"foo\":\"1\",\"bar\":{\"subcomponent\":{\"item\":[{\"eggs\":\"yum\"}]}}},{\"fiz\":\"baz\"}]}";
            Assert.Equal(expectedContent, transformedContent);
        }

        [Fact]
        public void TestRemoveInvalidPathNoNode() {
            string cmsContent = "{\"test\":[{\"foo\":\"bar\"},{\"fiz\":\"baz\"}]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "remove");
            transform.Add("jsonPath", "test");
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[{\"foo\":\"bar\"},{\"fiz\":\"baz\"}]}";
            Assert.Equal(expectedContent, transformedContent);
            Assert.Single(errors);
        }

        [Fact]
        public void TestRemoveInvalidPathWrongPath() {
            string cmsContent = "{\"test\":[{\"foo\":\"bar\"},{\"fiz\":\"baz\"}]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "remove");
            transform.Add("jsonPath", "blorenge[2]");
            transforms.Add(transform);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[{\"foo\":\"bar\"},{\"fiz\":\"baz\"}]}";
            Assert.Equal(expectedContent, transformedContent);
            Assert.Single(errors);
        }

        [Fact]
        public void TestMultipleUniqueTransforms() {
            string cmsContent = "{\"test\":[{\"foo\":\"bar\"},{\"fiz\":\"baz\"}]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "addToList");
            transform.Add("jsonPath", "test");
            var item = new Dictionary<string, object>();
            item.Add("first", "test");
            transform.Add("value", item);
            var transform2 = new Dictionary<string, object>();
            transform2.Add("method", "moveToIndex");
            transform2.Add("jsonPath", "test[2]");
            transform2.Add("value", 0);
            var transform3 = new Dictionary<string, object>();
            transform3.Add("method", "insertAfter");
            transform3.Add("jsonPath", "test[1]");
            var entry = new Dictionary<string, object>();
            entry.Add("another", "test");
            transform3.Add("value", entry);
            var transform4 = new Dictionary<string, object>();
            transform4.Add("method", "remove");
            transform4.Add("jsonPath", "test[1]");

            transforms.Add(transform);
            transforms.Add(transform2);
            transforms.Add(transform3);
            transforms.Add(transform4);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[{\"first\":\"test\"},{\"another\":\"test\"},{\"fiz\":\"baz\"}]}";
            Assert.Equal(expectedContent, transformedContent);
        }

        [Fact]
        public void TestMultipleUniqueTransformsWithErrors() {
            string cmsContent = "{\"test\":[{\"foo\":\"bar\"},{\"fiz\":\"baz\"}]}";
            var transforms = new List<Dictionary<string, object>>();
            var transform = new Dictionary<string, object>();
            transform.Add("method", "addToList");
            transform.Add("jsonPath", "test");
            var item = new Dictionary<string, object>();
            item.Add("first", "test");
            transform.Add("value", item);
            //Incorrect jsonPath
            var transform2 = new Dictionary<string, object>();
            transform2.Add("method", "moveToIndex");
            transform2.Add("jsonPath", "test[6]");
            transform2.Add("value", 0);
            //Incorrect method
            var transform3 = new Dictionary<string, object>();
            transform3.Add("method", "insertBefore");
            transform3.Add("jsonPath", "test[1]");
            var entry = new Dictionary<string, object>();
            entry.Add("another", "test");
            transform3.Add("value", entry);
            var transform4 = new Dictionary<string, object>();
            transform4.Add("method", "remove");
            transform4.Add("jsonPath", "test[1]");

            transforms.Add(transform);
            transforms.Add(transform2);
            transforms.Add(transform3);
            transforms.Add(transform4);

            var (transformedContent, errors) = CmsTransformLibrary.ApplyTransforms(cmsContent, transforms);
            string expectedContent = "{\"test\":[{\"foo\":\"bar\"},{\"first\":\"test\"}]}";
            Assert.Equal(expectedContent, transformedContent);
            List<string> expectedErrors = new List<string>(){
                "Node at 'test[6]' does not exist.",
                "insertBefore is not a valid method."
            };
            Assert.Equal(expectedErrors, errors);
        }
    }

}
